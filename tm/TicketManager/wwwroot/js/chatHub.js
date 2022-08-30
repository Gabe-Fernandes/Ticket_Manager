$(document).ready(function () {
  let chatWindows = JSON.parse(localStorage.getItem("chatWindows")) ?? [];
  let minimizedChatWindows = JSON.parse(localStorage.getItem("minimizedChatWindows")) ?? [];
  let usersInActiveWindows = JSON.parse(localStorage.getItem("usersInActiveWindows")) ?? [];
  let tempGuid = null;
  let tempCtx = null;
  let searchWindowIsOpen = localStorage.getItem("searchWindowIsOpen") ?? false;

  // Create/start connection
  var Messenger = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
  function Success() { console.log("success") }
  function Failure() { console.log("failure") }
  Messenger.start().then(Success, Failure);

  window.onload = () => {
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
  };

  //0
  $("#newChatBtn").on("click", () => {
    if (searchWindowIsOpen === false) {
      Messenger.send("GenerateChatGuidList");
    }
  });

  Messenger.on("RenderSearchWindow", chatGuidList => {
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
    Messenger.send("FilterUsers", filterString, usersInActiveWindows);
  }

  Messenger.on("UsersFiltered", filteredUsers => {
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
          `<li tabindex="0" class='search-result' id='${obj.id}'>
          <div><img src='${obj.profilePicture}'/>
          <p>${obj.firstName} ${obj.lastName}<br/>
            &nbsp;&nbsp;&nbsp; -${obj.assignedRole}`);
        $(`#${obj.id}`).on("click", ()=> { SelectCoworker(obj, guidList) });
      });
    }
  }

  //2
  function SelectCoworker(coworker, guidList) {
    const chatCtx = Object.assign(coworker, guidList);
    const index = CloseChatWindow(guidList);
    RenderChatWindow(chatCtx, index);
    tempCtx = chatCtx;
    Messenger.send("LoadMessages", chatCtx.id);
  }

  Messenger.on("MessagesLoaded", messages => {
    const chatCtx = tempCtx;
    const contentList = $(`#${chatCtx.contentListId}`)[0];
    contentList.innerHTML = "";
    messages.forEach(msg => {
      RenderMessage(msg, chatCtx);
    });
    contentList.scrollTop = contentList.scrollHeight;
    CheckForChatWindowShuffle();
  });

  function RenderMessage(msg, chatCtx) {
    if (msg.from === chatCtx.id) { // incoming message
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
    Messenger.send("SendMessage", chatCtx.id, $(`#${chatCtx.messageInputId}`)[0].value);
  }

  Messenger.on("MessageSent", msg => {
    const chatCtx = tempCtx;
    const contentList = $(`#${chatCtx.contentListId}`)[0];
    $(`#${chatCtx.messageInputId}`)[0].value = "";
    RenderMessage(msg, chatCtx);
    contentList.scrollTop = contentList.scrollHeight;
  });

  Messenger.on("MessageReceived", (guidList, coworker) => {
    const chatCtx = Object.assign(coworker, guidList);
    CloseMinimizedChatWindow(chatCtx); // does nothing if DNE
    RenderChatWindow(chatCtx);
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
      if (chatWindows[i].chatWindowId === chatCtx.chatWindowId) { // find the chat window to remove 
        if ($(`#${chatWindows[i].midId}`).hasClass("extend-mid")) { SetSearchWindowIsOpen(false) } // if deleting a search window
        else { usersInActiveWindows = usersInActiveWindows.filter(u => u != chatCtx.id) } // keep all users but the one at this window
        $(`#${chatCtx.chatWindowId}`)[0].remove();
        chatWindows.splice(i, 1);
        localStorage.setItem("chatWindows", JSON.stringify(chatWindows));
        return i;
      }
    }
  }

  function RenderMinimizedChatWindow(chatCtx) {
    $("#chatColumn").append(
      `<img id='${chatCtx.minimizedChatWindowId}' tabindex="0" src='${chatCtx.profilePicture}'>`);
    $(`#${chatCtx.minimizedChatWindowId}`).on("click", () => {
      CloseMinimizedChatWindow(chatCtx);
      RenderChatWindow(chatCtx);
    });
    if (minimizedChatWindows.length > 5) { // minimized window cap
      CloseMinimizedChatWindow(minimizedChatWindows[0]);
    }
  }

  function RenderChatWindow(chatCtx, index = chatWindows.length, loadingNewPage = false) {
    const renderString = `<div class='chat-window' id='${chatCtx.chatWindowId}'>
      <div class='top'>
        <img src='${chatCtx.profilePicture}' id='${chatCtx.pfpId}'>
        <label id='${chatCtx.nameTagId}'>${chatCtx.firstName} ${chatCtx.lastName}</label>
        <div><img tabindex="0" src='/icons/delete.png' id='${chatCtx.minimizeBtnId}'></div>
        <div><img tabindex="0" src='/icons/cancel.png' id='${chatCtx.closeBtnId}'></div></div>
      <div class='mid' id='${chatCtx.midId}'>
        <ol class='message-col' id='${chatCtx.contentListId}'></div>
      <div class='bot' id='${chatCtx.botId}'>
        <input placeholder='message...' id='${chatCtx.messageInputId}'><button
        id='${chatCtx.sendBtnId}' class='btn' tabindex="0">Send`


    if (index === chatWindows.length) {
      $("#chatContainer").append(renderString);
    }
    else {
      $(renderString).insertBefore(`#${chatWindows[index].chatWindowId}`);
    }

    if (loadingNewPage === false) {
      chatWindows.splice(index, 0, chatCtx); // replace searchwindow index or add to end
      localStorage.setItem("chatWindows", JSON.stringify(chatWindows));
      usersInActiveWindows.push(chatCtx.id);
      localStorage.setItem("usersInActiveWindows", JSON.stringify(usersInActiveWindows));
    }

    AddChatWindowEvents(chatCtx);
    tempCtx = chatCtx;
    Messenger.send("LoadMessages", chatCtx.id);
  }

  function AddChatWindowEvents(chatCtx) {
    $(`#${chatCtx.minimizeBtnId}`).on("click", () => { MinimizeChatWindow(chatCtx) });
    $(`#${chatCtx.sendBtnId}`).on("click", () => { SendBtn(chatCtx) });
    $(`#${chatCtx.closeBtnId}`).on("click", () => { CloseChatWindow(chatCtx); });
  }

  function SetSearchWindowIsOpen(bool) {
    searchWindowIsOpen = bool;
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
});
