$(document).ready(function () {
  let chatWindows = JSON.parse(localStorage.getItem("chatWindows")) ?? [];
  let minimizedChatWindows = JSON.parse(localStorage.getItem("minimizedChatWindows")) ?? [];
  let usersInActiveWindows = JSON.parse(localStorage.getItem("usersInActiveWindows")) ?? [];
  let tempGuid = null;
  let tempCtx = null;
  let searchWindowIsOpen = localStorage.getItem("searchWindowIsOpen") ?? "false";

  // Create/start connection
  var ChatMessenger = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
  var NavMessenger = new signalR.HubConnectionBuilder().withUrl("/navbarHub").build();
  function ChatSuccess() { console.log("chatHub success"); LoadStartupChats() }
  function NavSuccess() { console.log("navHub success") }
  function Failure() { console.log("failure") }
  ChatMessenger.start().then(ChatSuccess, Failure);
  ChatMessenger.onclose(async () => await ChatMessenger.start());
  NavMessenger.start().then(NavSuccess, Failure);
  NavMessenger.onclose(async () => await NavMessenger.start());

  function LoadStartupChats() {
    for (let i = 0; i < minimizedChatWindows.length; i++) {
      const chatCtx = minimizedChatWindows[i];
      RenderMinimizedChatWindow(chatCtx);
    }

    for (let i = 0; i < chatWindows.length; i++) {
      const chatCtx = chatWindows[i];

      if (("id" in chatCtx) === false) { // if search window
        RenderSearchWindow(chatCtx, true);
      }
      else {
        RenderChatWindow(chatCtx, chatWindows.length, true);
      }
    }
  }

  //0
  $("#newChatBtn").on("click", () => {
    if (searchWindowIsOpen === "false") {
      ChatMessenger.send("GenerateChatGuidList");
    }
  });
  $("#newChatBtn").hover(function () {
    $("#newChatTooltip").removeClass("hide");
  }, function () {
    $("#newChatTooltip").addClass("hide");
  });

  ChatMessenger.on("RenderSearchWindow", chatGuidList => {
    RenderSearchWindow(chatGuidList);
  });

  function RenderSearchWindow(guidList, loadingNewPage = false) {
    $("#chatContainer").append(`<div class='chat-window' id='${guidList.chatWindowId}'>
      <div class='top'>
        <input placeholder='to...' id='${guidList.searchBoxId}'>
        <div><img tabindex="0" src='/icons/cancel.png' id='${guidList.closeBtnId}'></div></div>
      <div class='mid extend-mid' id='${guidList.midId}'>
        <ol id='${guidList.contentListId}'>`);

    SetSearchWindowIsOpen(true);
    if (loadingNewPage === false) {
      chatWindows.push(guidList);
      localStorage.setItem("chatWindows", JSON.stringify(chatWindows));
    }
    $(`#${guidList.searchBoxId}`).on("input", () => { SearchBox(guidList) });
    $(`#${guidList.closeBtnId}`).on("click", () => { CloseChatWindow(guidList) });
    CheckForChatWindowShuffle();
  }

  //1
  function SearchBox(guidList) {
    const filterString = $(`#${guidList.searchBoxId}`)[0].value;
    tempGuid = guidList;
    ChatMessenger.send("FilterUsers", filterString, usersInActiveWindows);
  }

  ChatMessenger.on("UsersFiltered", filteredUsers => {
    RenderUsers(filteredUsers);
  });

  function RenderUsers(filteredUsers) {
    const guidList = tempGuid;
    $(`#${guidList.contentListId}`)[0].innerHTML = "";
    if (filteredUsers.length === 0) {
      $(`#${guidList.contentListId}`).append("<li>No team members to display...");
    }
    else {
      filteredUsers.forEach(obj => {
        $(`#${guidList.contentListId}`).append(
          `<li tabindex="0" class='search-result' id='${obj.selectUserBtnId}'>
          <div><img src='${obj.profilePicture}'/>
          <p>${obj.firstName} ${obj.lastName}<br/>
            &nbsp;&nbsp;&nbsp; -${obj.assignedRole}`);
        $(`#${obj.selectUserBtnId}`).on("click", ()=> { SelectCoworker(obj, guidList) });
      });
    }
  }

  //2
  function SelectCoworker(coworker, guidList) {
    const chatCtx = Object.assign(coworker, guidList);
    const index = CloseChatWindow(guidList);
    RenderChatWindow(chatCtx, index);
    ChatMessenger.send("LoadMessages", chatCtx.id);
  }

  ChatMessenger.on("MessagesLoaded", (messages, myId) => {
    const chatCtx = $(`#${myId}`)[0].value;
    const contentList = $(`#${chatCtx.contentListId}`)[0];
    contentList.innerHTML = "";
    messages.forEach(msg => {
      RenderMessage(msg, chatCtx);
    });
    contentList.scrollTop = contentList.scrollHeight;
    CheckForChatWindowShuffle(); // may not be needed anymore? -------------------------------
  });

  function RenderMessage(msg, chatCtx) {
    if (msg.senderId === chatCtx.id) { // incoming message
      $(`#${chatCtx.contentListId}`).prepend(`<li class='conversation'><div class='msg-in'>
      <div><img src='${chatCtx.profilePicture}'></div><p>${msg.body}`);
    }
    else { //outgoing message
      $(`#${chatCtx.contentListId}`).prepend(`<li class='conversation'><div class='msg-out'><p>${msg.body}`);
    }
  }

  //3
  function SendBtn(chatCtx) {
    tempCtx = chatCtx;
    ChatMessenger.send("SendMessage", chatCtx.id, $(`#${chatCtx.messageInputId}`)[0].value);
  }

  ChatMessenger.on("MessageSent", msg => {
    const chatCtx = tempCtx;
    const contentList = $(`#${chatCtx.contentListId}`)[0];
    $(`#${chatCtx.messageInputId}`)[0].value = "";
    RenderMessage(msg, chatCtx);
    contentList.scrollTop = contentList.scrollHeight;
  });

  ChatMessenger.on("MessageReceived", (guidList, coworker) => {
    const chatCtx = Object.assign(coworker, guidList);
    const index = CloseChatWindow(chatCtx); // does nothing if DNE
    CloseMinimizedChatWindow(chatCtx); // does nothing if DNE
    RenderChatWindow(chatCtx, index);
  });

  //4
  function MinimizeChatWindow(chatCtx) {
    CloseChatWindow(chatCtx);
    minimizedChatWindows.push(chatCtx);
    localStorage.setItem("minimizedChatWindows", JSON.stringify(minimizedChatWindows));
    usersInActiveWindows.push(chatCtx.id);
    localStorage.setItem("usersInActiveWindows", JSON.stringify(usersInActiveWindows));
    RenderMinimizedChatWindow(chatCtx);
  }
  
  function CloseMinimizedChatWindow(chatCtx) {
    for (let i = 0; i < minimizedChatWindows.length; i++) {
      if (minimizedChatWindows[i].id === chatCtx.id) { // find the chat window to remove 
        usersInActiveWindows = usersInActiveWindows.filter(u => u != chatCtx.id); // keep all users but the one at this window
        $(`#${minimizedChatWindows[i].minimizedChatWindowId}`)[0].remove();
        minimizedChatWindows.splice(i, 1);
        localStorage.setItem("minimizedChatWindows", JSON.stringify(minimizedChatWindows));
      }
    }
  }

  function CloseChatWindow(chatCtx) {
    for (let i = 0; i < chatWindows.length; i++) {
      if (chatWindows[i].id === chatCtx.id) { // find the chat window to remove 
        if ($(`#${chatWindows[i].midId}`).hasClass("extend-mid")) { // if deleting a search window
          SetSearchWindowIsOpen(false);
          $(`#${chatCtx.chatWindowId}`)[0].remove();
        }
        else {
          usersInActiveWindows = usersInActiveWindows.filter(u => u != chatCtx.id); // keep all users but the one at this window
          $(`#${chatCtx.id}`)[0].remove();
        }
        chatWindows.splice(i, 1);
        localStorage.setItem("chatWindows", JSON.stringify(chatWindows));
        return i;
      }
    }
  }

  function RenderMinimizedChatWindow(chatCtx) {
    $("#chatColumn").append(
      `<div id='${chatCtx.minimizedChatWindowId}'><img tabindex="0" src='${chatCtx.profilePicture}'>
        <span id='${chatCtx.nameTagId}' class="hide">${chatCtx.firstName} ${chatCtx.lastName}`);
    $(`#${chatCtx.minimizedChatWindowId}`).on("click", () => {
      CloseMinimizedChatWindow(chatCtx);
      RenderChatWindow(chatCtx);
    });
    $(`#${chatCtx.minimizedChatWindowId}`).hover(function () {
      $(`#${chatCtx.nameTagId}`).removeClass("hide");
    }, function () {
      $(`#${chatCtx.nameTagId}`).addClass("hide");
    });
    if (minimizedChatWindows.length > 5) { // minimized window cap
      CloseMinimizedChatWindow(minimizedChatWindows[0]);
    }
  }

  function RenderChatWindow(chatCtx, index = chatWindows.length, loadingNewPage = false) {
    const renderString = `<div class='chat-window' id='${chatCtx.id}'>
      <div class='top'>
        <img src='${chatCtx.profilePicture}' id='${chatCtx.pfpId}'>
        <label id='${chatCtx.nameTagId}'>${chatCtx.firstName} ${chatCtx.lastName}</label>
        <div><img tabindex="0" src='/icons/delete.png' id='${chatCtx.minimizeBtnId}'></div>
        <div><img tabindex="0" src='/icons/cancel.png' id='${chatCtx.closeBtnId}'></div></div>
      <div class='mid' id='${chatCtx.midId}'>
        <ol class='message-col' id='${chatCtx.contentListId}'></div>
      <div class='bot' id='${chatCtx.botId}'>
        <input placeholder='message...' id='${chatCtx.messageInputId}'><button
        id='${chatCtx.sendBtnId}' class='btn' tabindex="0">Send</button></div>`

    if (index === chatWindows.length) {
      $("#chatContainer").append(renderString);
    }
    else {
      const id = chatWindows[index].id ?? chatWindows[index].chatWindowId;
      $(renderString).insertBefore(`#${id}`);
    }

    if (loadingNewPage === false) {
      chatWindows.splice(index, 0, chatCtx); // replace searchwindow index or add to end
      localStorage.setItem("chatWindows", JSON.stringify(chatWindows));
      usersInActiveWindows.push(chatCtx.id);
      localStorage.setItem("usersInActiveWindows", JSON.stringify(usersInActiveWindows));
    }

    AddChatWindowEvents(chatCtx);
    $(`#${chatCtx.id}`)[0].value = chatCtx;
    ChatMessenger.send("LoadMessages", chatCtx.id);
  }

  function AddChatWindowEvents(chatCtx) {
    $(`#${chatCtx.minimizeBtnId}`).on("click", () => { MinimizeChatWindow(chatCtx) });
    $(`#${chatCtx.sendBtnId}`).on("click", () => { SendBtn(chatCtx) });
    $(`#${chatCtx.closeBtnId}`).on("click", () => { CloseChatWindow(chatCtx); });
  }

  function SetSearchWindowIsOpen(bool) {
    searchWindowIsOpen = (bool) ? "true" : "false";
    localStorage.setItem("searchWindowIsOpen", searchWindowIsOpen);
  }

  function CheckForChatWindowShuffle() {
    if (chatWindows.length > 3) { // minimized window cap
      if (("id" in chatWindows[0]) === false) { // if search window
        CloseChatWindow(chatWindows[0]);
      }
      else {
        MinimizeChatWindow(chatWindows[0]);
      }
    }
  }






  $(document).click(function (event) { //close navbar panel when clicking anywhere not listed here
    const $target = $(event.target);
    if (!$target.closest("#notificationPanel").length &&
      !$target.closest("#chatPanel").length &&
      !$target.closest("#userPanel").length &&
      !$target.closest("#notificationMenuBtn").length &&
      !$target.closest("#chatMenuBtn").length &&
      !$target.closest("#userMenuBtn").length) {
      ClearPanels();
    }
  });

  function ClearPanels() {
    $("#notificationPanel").addClass("hide");
    $("#chatPanel").addClass("hide");
    $("#userPanel").addClass("hide");
    $("#chatSearch")[0].value = "";
    $("#notificationSearch")[0].value = "";
    showMoreIndex = 0;
  }

  function SwitchNavbarPanel(myPanel, panel1, panel2) {
    if ($(`#${myPanel}`).hasClass("hide")) { //open panel
      $(`#${myPanel}`).removeClass("hide");
      $(`#${panel1}`).addClass("hide");
      $(`#${panel2}`).addClass("hide");

      $("#moreNotificationsBtn").removeClass("hide");
      $("#moreChatsBtn").removeClass("hide");
      NavMessenger.send("GetData", "chatContent", true, 0); // generalize later ----------------------------
    }
    else { //close panel
      $(`#${myPanel}`).addClass("hide");
      showMoreIndex = 0;
    }
  }

  $("#userMenuBtn").on("click", () => {
    SwitchNavbarPanel("userPanel", "notificationPanel", "chatPanel");
  });

  // notification events
  $("#notificationMenuBtn").on("click", () => {
    SwitchNavbarPanel("notificationPanel", "chatPanel", "userPanel");
  });
  $("#moreNotificationsBtn").on("click", () => {
    showMoreIndex += 5;
    NavMessenger.send("GetData", "notificationContent", false, showMoreIndex);
  });
  $("#notificationSearch").on("input", () => {
    $("#moreNotificationsBtn").addClass("hide");
    showMoreIndex = 0;
    const filterString = $("#notificationSearch")[0].value;
    NavMessenger.send("FilterContent", "notificationContent", filterString);
  });

  // chat events
  $("#chatMenuBtn").on("click", () => {
    SwitchNavbarPanel("chatPanel", "notificationPanel", "userPanel");
  });
  $("#moreChatsBtn").on("click", () => {
    showMoreIndex += 5;
    NavMessenger.send("GetData", "chatContent", false, showMoreIndex);
  });
  $("#chatSearch").on("input", () => {
    $("#moreChatsBtn").addClass("hide");
    showMoreIndex = 0;
    const filterString = $("#chatSearch")[0].value;
    NavMessenger.send("FilterContent", "chatContent", filterString);
  });

  // process server data
  NavMessenger.on("PanelDataReceiver", (contentId, panelData, clearContent, hideShowMoreBtn) => {
    if (hideShowMoreBtn) {
      $("#moreNotificationsBtn").addClass("hide");
      $("#moreChatsBtn").addClass("hide");
    }
    else {
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
      <div tabindex="0" id="${panelData[i].selectNavMenuOptionId}" class="row">
        <img src="${panelData[i].imgSrc}">
        <div>
          <span>${panelData[i].title}<br>
          ${panelData[i].description}<br>
          ${panelData[i].time}`);
      if (contentId === "chatContent") {
        $(`#${panelData[i].selectNavMenuOptionId}`).on("click", () => {
          ClearPanels();
          ChatMessenger.send("OpenChatFromNav", panelData[i].coworkerId);
        });
      }
    }
  }
});
