$(document).ready(function () {
  const hide = "hide";
  const unclickable = "unclickable";

  var TechLeadDashConnection = new signalR.HubConnectionBuilder().withUrl("/tleadDashHub").build();
  function Success() { console.log("TechLeadDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  TechLeadDashConnection.start().then(Success, Failure);
  TechLeadDashConnection.onclose(async () => await TechLeadDashConnection.start());

  function PageLoad() {
    TechLeadDashConnection.send("LoadTickets");
    TechLeadDashConnection.send("LoadTeamMembers");
  }

  TechLeadDashConnection.on("GetTickets", ticketCtxList => {
    ticketCtxList.forEach(ctx => {
      RenderTickets(ctx);
    });
  });

  function RenderTickets(ctx) {

  }

  TechLeadDashConnection.on("GetTeamMembers", memberCtxList => {
    memberCtxList.forEach(ctx => {
      RenderTeamMember(ctx);
    });
  });

  function RenderTeamMember(ctx) {

  }

  $("#ticketSearchTxt").on("click", () => {

  });
  $("#newTicketBtn").on("click", () => {
    $("#newTicketModal").removeClass(hide);
    $("#tleadDashMain").addClass(unclickable);
  });
  $("#assignBtn").on("click", () => {
    $("#newTicketModal").addClass(hide);
    $("#tleadDashMain").removeClass(unclickable);
  });
  $("#closeBtn").on("click", () => {
    $("#newTicketModal").addClass(hide);
    $("#tleadDashMain").removeClass(unclickable);
  });
});
