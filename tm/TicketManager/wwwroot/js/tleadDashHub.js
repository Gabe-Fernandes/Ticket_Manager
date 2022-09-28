$(document).ready(function () {
  const hide = "hide";
  const unclickable = "unclickable";
  let currentTicket = "";

  var TechLeadDashConnection = new signalR.HubConnectionBuilder().withUrl("/tleadDashHub").build();
  function Success() { console.log("TechLeadDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  TechLeadDashConnection.start().then(Success, Failure);
  TechLeadDashConnection.onclose(async () => await TechLeadDashConnection.start());

  function PageLoad() {
    TechLeadDashConnection.send("LoadTickets", ""); //no filter
  }

  TechLeadDashConnection.on("GetTickets", ticketList => {
    ticketList.forEach(ticket => {
      RenderTicket(ticket);
    });
  });

  function RenderTicket(ticket) {
    $("#ticketTbody").append(`
			<tr id="${ticket.tableRowId}">
				<td>${ticket.title}</td>
				<td>${ticket.description}</td>
				<td><div class="${ticket.status}"><div></div><div></div></div></td>
				<td>${ticket.priorityLevel}</td>
				<td>${ticket.startDate}</td>
				<td>${ticket.endDate}</td>
				<td>${ticket.senderName}</td>"
				<td>${ticket.recipientName}</td>
				<td class="btn-td"><button id="${ticket.detailsBtnId}" class="btn" tabindex="0">details</button></td>
			</tr>
    `);
    $(`#${ticket.detailsBtnId}`).on("click", () => {
      $("#tleadDashMain").addClass(hide);
      $("#ticketDetailsMain").removeClass(hide);
      currentTicket = ticket.tableRowId;
      PopulateTicketDetails(ticket);
    });
  }

  TechLeadDashConnection.on("UpdateTicket", ticket => {
    if (ticket.tableRowId === currentTicket) {
      PopulateTicketDetails(ticket);
    }
    $(`#${ticket.tableRowId}`)[0].remove();
    RenderTicket(ticket);
  });

  function PopulateTicketDetails(ticket) {
    $("#detailTitle").text(ticket.title);
    $("#detailPriority").text(`(Priority ${ticket.priorityLevel})`);
    $("#detailStatus").removeClass();
    $("#detailStatus").addClass(ticket.status);
    $("#detailSenderPfp").attr("src", ticket.senderPfp);
    $("#detailStartDate").text(`Assigned ${ticket.startDate}`);
    $("#detailRecipientPfp").attr("src", ticket.recipientPfp);
    $("#detailEndDate").text(`Due ${ticket.endDate}`);
    $("#detailDescription").text(ticket.description);
    $(`#editBtn`).on("click", () => {
      $("#updateTicketModal").removeClass(hide);
      $("#ticketDetailsMain").addClass(unclickable);
      PopulateEditModal(ticket);
    });
    $("#deleteBtn").on("click", () => {
      $("#tleadDashMain").removeClass(hide);
      $("#ticketDetailsMain").addClass(hide);
      TechLeadDashConnection.send("DeleteTicket", ticket.id);
    });
    $("#approveBtn").on("click", () => {
      TechLeadDashConnection.send("ApproveTicket", ticket.id);
    });
  }
  TechLeadDashConnection.on("DeleteTicket", ticket => {
    $(`#${ticket.tableRowId}`)[0].remove();
  });

  function PopulateEditModal(ticket) {
    $("#updateTitle").val(ticket.title);
    $("#updateDescription").text(ticket.description);
    $("#updateStatus").val(ticket.status);
    $("#updatePriority").val(ticket.priorityLevel);
    $("#updateDueDate").text(`Due Date: ${ticket.endDate}`);
    TechLeadDashConnection.send("LoadTeamMembers", ticket.recipientId);

    $("#updateBtn").on("click", () => {
      $("#updateTicketModal").addClass(hide);
      $("#ticketDetailsMain").removeClass(unclickable);
      const formData = {
        title: $("#updateTitle")[0].value,
        description: $("#updateDescription")[0].value,
        status: $("#updateStatus")[0].value,
        priorityLevel: $("#updatePriority")[0].value,
        tempDate: $("#updateDueDateInput")[0].value,
        recipientId: document.querySelector('input[name="user"]:checked').value
      }
      TechLeadDashConnection.send("UpdateTicket", formData, ticket.id);
    });
  }

  TechLeadDashConnection.on("GetTeamMembers", memberCtxList => {
    $("#teamMemberList")[0].innerHTML = "";
    $("#updateTeamMemberList")[0].innerHTML = "";
    memberCtxList.forEach(ctx => {
      RenderTeamMember(ctx);
    });
  });

  function RenderTeamMember(ctx) {
    const renderString = 
      `<div id="${ctx.userDivId}" class="user">
				<input id="${ctx.userRadioBtnId}" value="${ctx.recipientId}" type="radio" name="user">
				<img src="${ctx.pfp}">
				<span>${ctx.firstName} ${ctx.lastName}</span>
			</div>`
    if (ctx.modalType === "create") {
      $("#teamMemberList").append(renderString);
    }
    else {
      $("#updateTeamMemberList").append(renderString);
      if (ctx.modalType === ctx.recipientId) { //update modaltype is recipientId
        $(`#${ctx.userRadioBtnId}`).attr("checked", "checked");
      }
    }

    $(`#${ctx.userDivId}`).on("click", () => {
      $(`#${ctx.userRadioBtnId}`)[0].checked = "checked";
    });
  }

  $("#ticketSearchTxt").on("input", () => {
    TechLeadDashConnection.send("LoadTickets", $("#ticketSearchTxt")[0].value);
  });
  $("#newTicketBtn").on("click", () => {
    $("#newTicketModal").removeClass(hide);
    $("#tleadDashMain").addClass(unclickable);
    TechLeadDashConnection.send("LoadTeamMembers", "create");
  });
  $("#assignBtn").on("click", () => {
    $("#newTicketModal").addClass(hide);
    $("#tleadDashMain").removeClass(unclickable);
    const formData = {
      title: $("#tktTitle")[0].value,
      description: $("#tktDescription")[0].value,
      status: $("#tktStatus")[0].value,
      priorityLevel: $("#tktPriority")[0].value,
      tempDate: $("#dueDate")[0].value,
      recipientId: document.querySelector('input[name="user"]:checked').value
    }
    TechLeadDashConnection.send("CreateTicket", formData);
  });
  $("#closeBtn").on("click", () => {
    $("#newTicketModal").addClass(hide);
    $("#tleadDashMain").removeClass(unclickable);
  });
  $(`#ticketBackBtn`).on("click", () => {
    $("#tleadDashMain").removeClass(hide);
    $("#ticketDetailsMain").addClass(hide);
    currentTicket = "";
  });
  $("#updateCloseBtn").on("click", () => {
    $("#updateTicketModal").addClass(hide);
    $("#ticketDetailsMain").removeClass(unclickable);
  });
});
