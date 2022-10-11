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

  // main page ---------------------------------------------------------------------------
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
      TechLeadDashConnection.send("LoadComments", ticket.id);
    });
  }
  $("#ticketSearchTxt").on("input", () => {
    TechLeadDashConnection.send("LoadTickets", $("#ticketSearchTxt")[0].value);
  });
  $("#newTicketBtn").on("click", () => {
    ClearNewTicketForm();
    $("#newTicketModal").removeClass(hide);
    $("#tleadDashMain").addClass(unclickable);
    TechLeadDashConnection.send("LoadTeamMembers", "create");
  });

  //modal ---------------------------------------------------------------------------
  function ClearNewTicketForm() {
    $("#tktTitle")[0].value = "";
    $("#tktDescription")[0].value = "";
    $("#tktStatus")[0].value = "Open";
    $("#tktPriority")[0].value = "Low";
    $("#dueDate")[0].value = Date.now();
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

  // ticket details ---------------------------------------------------------------------------
  TechLeadDashConnection.on("UpdateTicket", ticket => {
    if (ticket.tableRowId === currentTicket) {
      PopulateTicketDetails(ticket);

    }
    $(`#${ticket.tableRowId}`)[0].remove();
    RenderTicket(ticket);
  });

  TechLeadDashConnection.on("DeleteTicket", ticket => {
    if (ticket.tableRowId === currentTicket) {
      $("#tleadDashMain").removeClass(hide);
      $("#ticketDetailsMain").addClass(hide);
      $("#comments")[0].innerHTML = "";
    }
    $(`#${ticket.tableRowId}`)[0].remove();
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
    TicketDetailsBtns(ticket);
  }

  function TicketDetailsBtns(ticket) {
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
    $("#commentTxt").on("input", () => { CommentTxt() });
    $("#commentBtn").on("click", () => { CommentBtn(ticket) });
  }

  $(`#ticketBackBtn`).on("click", () => {
    $("#tleadDashMain").removeClass(hide);
    $("#ticketDetailsMain").addClass(hide);
    currentTicket = "";
    $("#comments")[0].innerHTML = "";
    ClearDetailEvents();
  });

  function ClearDetailEvents() {
    $(`#editBtn`).off();
    $("#deleteBtn").off();
    $("#approveBtn").off();
    $("#commentTxt").off();
    $("#commentBtn").off();
  }

  // edit modal ---------------------------------------------------------------------------
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
      $("#updateBtn").off();
      TechLeadDashConnection.send("UpdateTicket", formData, ticket.id);
    });
  }

  $("#updateCloseBtn").on("click", () => {
    $("#updateTicketModal").addClass(hide);
    $("#ticketDetailsMain").removeClass(unclickable);
    $("#updateBtn").off();
  });

  // comments ---------------------------------------------------------------------------
  function CommentTxt() {
    if ($("#commentTxt")[0].value === "") {
      $("#commentBtn").addClass("disable");
    }
    else {
      $("#commentBtn").removeClass("disable");
    }
  }

  function CommentBtn(ticket) {
    const commentBody = $("#commentTxt")[0].value;
    $("#commentTxt")[0].value = "";
    $("#commentBtn").addClass("disable");
    TechLeadDashConnection.send("PostComment", commentBody, ticket.id, ticket.tableRowId);
  }

  TechLeadDashConnection.on("PostComment", commentList => {
    if (commentList[0].tableRowId === currentTicket) { // if this comment's ticket is the ticket being viewed currently
      commentList.forEach(comment => {
        RenderComment(comment);
      });
      $("#comments")[0].scrollTop = $("#comments")[0].scrollHeight;
    }
  });

  function RenderComment(comment) {
    $("#comments").append(`
      <div class="comment">
        <div class="info">
          <div class="top">
            <img src="${comment.pfp}">
            <span>${comment.firstName}<br>${comment.lastName}</span>
          </div>
          <div class="bot">
            <span>${comment.date}</span>
          </div>
        </div>
        <div class="body">
          <p>${comment.body}</p>
        </div>
      </div>
    `);
  }
});
