$(document).ready(function () {
  const hide = "hide";
  const unclickable = "unclickable";

  var TechLeadDashConnection = new signalR.HubConnectionBuilder().withUrl("/tleadDashHub").build();
  function Success() { console.log("TechLeadDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  TechLeadDashConnection.start().then(Success, Failure);
  TechLeadDashConnection.onclose(async () => await TechLeadDashConnection.start());

  function PageLoad() {
    TechLeadDashConnection.send("LoadTickets", ""); //no filter
  }

  TechLeadDashConnection.on("GetTickets", (ticketList, detailsBtnId) => {
    ticketList.forEach(ticket => {
      RenderTickets(ticket, detailsBtnId);
    });
  });

  function RenderTickets(ticket, detailsBtnId) {
    $("#ticketTbody").append(`
			<tr>
				<td>${ticket.title}</td>
				<td>${ticket.description}</td>
				<td><div class="${ticket.status}"><div></div><div></div></div></td>
				<td>${ticket.priorityLevel}</td>
				<td>${ticket.startDate}</td>
				<td>${ticket.endDate}</td>
				<td>${ticket.senderName}</td>
				<td>${ticket.recipientName}</td>
				<td class="btn-td"><button id="${detailsBtnId}" class="btn" tabindex="0">details</button></td>
			</tr>
    `);
    $(`#${detailsBtnId}`).on("click", () => {

    });
  }

  TechLeadDashConnection.on("GetTeamMembers", memberCtxList => {
    memberCtxList.forEach(ctx => {
      RenderTeamMember(ctx);
    });
  });

  function RenderTeamMember(ctx) {
    $("#teamMemberList").append(`
			<div id="${ctx.userDivId}" class="user">
				<input id="${ctx.userRadioBtnId}" value="${ctx.recipientId}" type="radio" name="user">
				<img src="${ctx.pfp}">
				<span>${ctx.firstName} ${ctx.lastName}</span>
			</div>
    `);
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
    TechLeadDashConnection.send("LoadTeamMembers");
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
});
