$(document).ready(function () {
  let chatWindows = [];
  let minimizedChatWindows = [];
  let tempGuid = null;
  let tempCtx = null;

  // Create connection
  var Messenger = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

  //0
  $("#newChatBtn").on("click", () => {
    if (chatWindows.length > 2) {
      ShuffleChatWindows();
    }
    Messenger.send("GenerateChatGuidList");
  });

  Messenger.on("RenderChatWindow", chatGuidList => {
    RenderChatWindow(chatGuidList);
  });

  function RenderChatWindow(guidList) {
    $("#chatContainer").append(`<div class='chat-window' id='${guidList.chatWindowId}'>
      <div class='top'>
        <img class='hide pfp' src='/icons/user.png' id='${guidList.pfpId}'>
        <label id='${guidList.nameTagId}'></label><input placeholder='to...' id='${guidList.searchBoxId}'>
        <img class='minimize' src='/icons/delete.png' id='${guidList.minimizeBtnId}'>
        <img class='close' src='/icons/cancel.png' id='${guidList.closeBtnId}'></div>
      <div class='mid'>
        <ol id='${guidList.contentListId}'></div>
      <div class='bot'>
        <input placeholder='message...' id='${guidList.messageInputId}'><button
        id='${guidList.sendBtnId}'>Send`);

    chatWindows.push(guidList);
    AddChatWindowEvents(guidList);
  }

  function AddChatWindowEvents(guidList) {
    $(`#${guidList.searchBoxId}`).on("input", function () { SearchBox(guidList) });
    $(`#${guidList.minimizeBtnId}`).on("click", function () { MinimizeChatWindow(guidList) });
    $(`#${guidList.closeBtnId}`).on("click", function () { CloseChatWindow(guidList) });
    $(`#${guidList.sendBtnId}`).on("click", function () { SendBtn(guidList) });
  }//()=>

  //1
  function SearchBox(guidList) {
    const filterString = $(`#${guidList.searchBoxId}`)[0].value;
    tempGuid = guidList;
    Messenger.send("FilterUsers", filterString);
  }

  Messenger.on("UsersFiltered", (filteredUsers) => {
    RenderUsers(filteredUsers);
  });

  function RenderUsers(filteredUsers) {
    const guidList = tempGuid;
    $(`#${guidList.contentListId}`)[0].innerHTML = "";
    const contentList = guidList.contentListId;
    if (filteredUsers.length === 0) {
      $(`#${contentList}`).append("<li>No team members to display...");
    }
    else {
      filteredUsers.forEach(obj => {
        $(`#${contentList}`).append(
          `<li class='search-result' id='${obj.id}'>
          <div><img src='${obj.profilePicture}'/>
          <p>${obj.firstName} ${obj.lastName}<br/>
            &nbsp;&nbsp;&nbsp; -${obj.assignedRole}`);
        $(`#${obj.id}`).on("click", function () { SelectCoworker(obj, guidList) });
      });
    }
  }

  //2
  function SelectCoworker(coworker, guidList) {
    const chatCtx = Object.assign(coworker, guidList);
    $(`#${chatCtx.chatWindowId}`)[0].value = chatCtx;
    tempCtx = chatCtx;
    SwitchHeaderFromSearchToChat(chatCtx);
    Messenger.send("LoadMessages", chatCtx.id);
  }

  function SwitchHeaderFromSearchToChat(chatCtx) {
    $(`#${chatCtx.searchBoxId}`).attr({ value: "", placeholder: "", class: "disable" });
    $(`#${chatCtx.nameTagId}`)[0].innerHTML = `${chatCtx.firstName} ${chatCtx.lastName}`;
    $(`#${chatCtx.pfpId}`).attr({ class: "pfp", src: `${chatCtx.profilePicture}` });
  }

  Messenger.on("MessagesLoaded", messages => {
    const chatCtx = tempCtx;
    const contentList = $(`#${chatCtx.contentListId}`)[0];
    contentList.innerHTML = "";
    messages.forEach(msg => {
      RenderMessage(msg, chatCtx);
    });
    contentList.scrollTop = contentList.scrollHeight;
  });

  function RenderMessage(msg, chatCtx) {
    if (msg.from === chatCtx.id) {
      const pfpSrc = $(`#${chatCtx.pfpId}`)[0].getAttribute('src');

      $(`#${chatCtx.contentListId}`).append(`<li class='conversation'><div class='msg-in'>
      <div><img src='${pfpSrc}'></div><p>${msg.body}`);
    }
    else {
      $(`#${chatCtx.contentListId}`).append(`<li class='conversation'><div class='msg-out'><p>${msg.body}`);
    }
  }

  //3
  function SendBtn(guidList) {
    const chatCtx = $(`#${guidList.chatWindowId}`)[0].value;
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

  function MinimizeChatWindow(guidList) {

  }

  function CloseChatWindow(guidList) {

  }

  function ShuffleChatWindows() {

  }

  function RenderMinimizedChatWindow() {

  }

  // Start connection
  function Success() { console.log("success") }
  function Failure() { console.log("failure") }
  Messenger.start().then(Success, Failure);

});
