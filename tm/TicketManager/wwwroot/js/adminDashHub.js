$(document).ready(function () {
  var AdminDashConnection = new signalR.HubConnectionBuilder().withUrl("/adminDashHub").build();
  function Success() { console.log("AdminDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  AdminDashConnection.start().then(Success, Failure);
  AdminDashConnection.onclose(async () => await AdminDashConnection.start());

  function PageLoad() {

  }
});
