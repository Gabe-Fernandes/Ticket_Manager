$(document).ready(function () {
  // Create/start connection
  var NavMessenger = new signalR.HubConnectionBuilder().withUrl("/navbarHub").build();
  function Success() { console.log("success") }
  function Failure() { console.log("failure") }
  NavMessenger.start().then(Success, Failure);

  $("#userMenuBtn").on("click", () => {
    SwitchNavbarPanel("userPanel", "notificationPanel", "chatPanel", true);
  });

  $(document).click(function (event) { //close navbar panel when clicking anywhere not listed here
    const $target = $(event.target);
    if (!$target.closest("#navbarPanel").length &&
      !$target.closest("#notificationMenuBtn").length &&
      !$target.closest("#chatMenuBtn").length &&
      !$target.closest("#userMenuBtn").length) {
      $("#navbarPanel").addClass("hide");
      $("#notificationPanel").addClass("hide");
      $("#chatPanel").addClass("hide");
      $("#userPanel").addClass("hide");
    }
  });

  function SwitchNavbarPanel(myPanel, panel1, panel2, smallPanel = false) {
    if ($(`#${myPanel}`).hasClass("hide")) { //open panel
      $("#navbarPanel").removeClass("hide");
      $("#moreNotificationsBtn").removeClass("hide");
      $("#moreChatsBtn").removeClass("hide");
      if (smallPanel) { $("#navbarPanel").addClass("small-panel") }
      else { $("#navbarPanel").removeClass("small-panel") }
      $(`#${myPanel}`).removeClass("hide");
      $(`#${panel1}`).addClass("hide");
      $(`#${panel2}`).addClass("hide");
      NavMessenger.send("GetData", "chatContent", true, false); // generalize later ----------------------------
    }
    else { //close panel
      $("#navbarPanel").addClass("hide");
      $(`#${myPanel}`).addClass("hide");
    }
  }

  // notification events
  $("#notificationMenuBtn").on("click", () => {
    SwitchNavbarPanel("notificationPanel", "chatPanel", "userPanel");
  });
  $("#moreNotificationsBtn").on("click", () => {
    NavMessenger.send("GetData", "notificationContent", false, false);
  });
  $("#notificationSearch").on("input", () => {
    $("#moreNotificationsBtn").addClass("hide");
    const filterString = $("#notificationSearch")[0].value;
    NavMessenger.send("FilterContent", "notificationContent", filterString);
  });

  // chat events
  $("#chatMenuBtn").on("click", () => {
    SwitchNavbarPanel("chatPanel", "notificationPanel", "userPanel");
  });
  $("#moreChatsBtn").on("click", () => {
    NavMessenger.send("GetData", "chatContent", false, false);
  });
  $("#chatSearch").on("input", () => {
    $("#moreChatsBtn").addClass("hide");
    const filterString = $("#chatSearch")[0].value;
    NavMessenger.send("FilterContent", "chatContent", filterString);
  });

  // process server data
  NavMessenger.on("PanelDataReceiver", (contentId, panelData, clearContent, calledFromSearchMethod) => {
    if (calledFromSearchMethod) {
      $("#moreNotificationsBtn").removeClass("hide");
      $("#moreChatsBtn").removeClass("hide");
    }
    RenderContent(contentId, panelData, clearContent);
  });

  function RenderContent(contentId, panelData, clearContent) {
    if (clearContent) {
      $(`#${contentId}`)[0].innerHTML = "";
    }
    if (panelData.length === 0) {
      $(`#${contentId}`).append("<span>No conversations to display...");
    }
    for (let i = 0; i < panelData.length; i++) {
      $(`#${contentId}`).append(`
      <div tabindex="0" class="row">
        <img src="${panelData[i].imgSrc}">
        <div>
          <span>${panelData[i].title}<br>
          ${panelData[i].description}<br>
          ${panelData[i].time}`);
    }
  }
});
