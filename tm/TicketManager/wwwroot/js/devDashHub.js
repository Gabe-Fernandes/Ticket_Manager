$(document).ready(function () {
  var DevDashConnection = new signalR.HubConnectionBuilder().withUrl("/devDashHub").build();
  function Success() { console.log("DevDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  DevDashConnection.start().then(Success, Failure);
  DevDashConnection.onclose(async () => await DevDashConnection.start());

  function PageLoad() {
    DevDashConnection.send("LoadMyTickets", ""); //no filter
  }

  $("#myTicketSearchTxt").on("input", () => {
    DevDashConnection.send("LoadMyTickets", $("#myTicketSearchTxt")[0].value);
  });

  DevDashConnection.on("GetMyTickets", ticketList => {
    $("#ticketTbody")[0].innerHTML = "";
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
				<td class="btn-td"><button id="${ticket.detailsBtnId}" class="btn" tabindex="0"></button></td>
			</tr>
    `);
    $(`#${ticket.detailsBtnId}`)[0].innerHTML = (ticket.status === "Open") ? "submit" : "reopen";

    $(`#${ticket.detailsBtnId}`).on("click", () => {
      if (ticket.status === "Open") {
        DevDashConnection.send("SubmitOrReopenTicket", ticket.id, "Review");
        $(`#${ticket.detailsBtnId}`)[0].innerHTML = "reopen";
      }
      else {
        DevDashConnection.send("SubmitOrReopenTicket", ticket.id, "Open");
        $(`#${ticket.detailsBtnId}`)[0].innerHTML = "submit";
      }
    });
  }
});
