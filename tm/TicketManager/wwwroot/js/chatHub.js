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
      //push
    }
    Messenger.send("GenerateChatGuidList");
  });

  Messenger.on("RenderNewChatWindow", chatGuidList => {
    RenderNewChatWindow(chatGuidList);
  });

  function RenderNewChatWindow(guidList) {
    $("#chatContainer").append(`<div class='chat-window' id='${guidList.chatWindowId}'>
      <div class='top'>
        <img class='hide' src='/icons/user.png' id='${guidList.pfpId}'>
        <label class='hide' id='${guidList.nameTagId}'></label>
        <input placeholder='to...' id='${guidList.searchBoxId}'>
        <div><img src='/icons/cancel.png' id='${guidList.closeBtnId}'></div></div>
      <div class='mid extend-mid' id='${guidList.midId}'>
        <ol id='${guidList.contentListId}'></div>
      <div class='bot hide' id='${guidList.botId}'>
        <input placeholder='message...' id='${guidList.messageInputId}'><button
        id='${guidList.sendBtnId}' class='btn'>Send`);

    chatWindows.push(guidList);
    AddChatWindowEvents(guidList);
  }

  function AddChatWindowEvents(guidList) {
    $(`#${guidList.searchBoxId}`).on("input", () => { SearchBox(guidList) });
    $(`#${guidList.sendBtnId}`).on("click", () => { SendBtn(guidList) });
    $(`#${guidList.closeBtnId}`).on("click", () => {
      ToggledChatWindow(guidList.chatWindowId, chatWindows, "chatWindowId");
    });
  }

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
        $(`#${obj.id}`).on("click", ()=> { SelectCoworker(obj, guidList) });
      });
    }
  }

  //2
  function SelectCoworker(coworker, guidList) {
    const chatCtx = Object.assign(coworker, guidList);
    $(`#${chatCtx.chatWindowId}`)[0].value = chatCtx; //is this storage persistent
    tempCtx = chatCtx;
    SwitchHeaderFromSearchToChat(chatCtx);
    SwitchFooterFromSearchToChat(chatCtx);
    Messenger.send("LoadMessages", chatCtx.id);
  }

  function SwitchHeaderFromSearchToChat(chatCtx) {
    $(`#${chatCtx.searchBoxId}`).attr("class", "hide");
    $(`#${chatCtx.nameTagId}`).attr("class", '');
    $(`#${chatCtx.nameTagId}`)[0].innerHTML = `${chatCtx.firstName} ${chatCtx.lastName}`;
    $(`#${chatCtx.pfpId}`).attr({ src: `${chatCtx.profilePicture}`, class: '' });

    $(`<div><img src='/icons/delete.png' id='${chatCtx.minimizeBtnId}'></div>`).insertAfter(`#${chatCtx.searchBoxId}`);
    $(`#${chatCtx.minimizeBtnId}`).on("click", () => { MinimizeChatWindow(chatCtx) });
  }

  function SwitchFooterFromSearchToChat(chatCtx) {
    $(`#${chatCtx.midId}`).attr("class", "mid");
    $(`#${chatCtx.botId}`).attr("class", "bot");
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

  //4
  function MinimizeChatWindow(chatCtx) {
    const minimizedWindow = ToggledChatWindow(chatCtx.chatWindowId, chatWindows, "chatWindowId");
    minimizedChatWindows.push(minimizedWindow);
    RenderMinimizedChatWindow(chatCtx);
  }
  
  function ToggledChatWindow(windowId, chatArray, idType) {
    $(`#${windowId}`)[0].remove();
    for (let i = 0; i < chatArray.length; i++) {
      if (chatArray[i][idType] === windowId) {
        return chatArray.splice(i, 1);
      }
    }
  }

  function RenderMinimizedChatWindow(chatCtx) {
    $("#chatColumn").append(`<img id='${chatCtx.minimizedChatWindowId}' src='${chatCtx.profilePicture}'>`);
    $(`#${chatCtx.minimizedChatWindowId}`).on("click", () => {
      ToggledChatWindow(chatCtx.minimizedChatWindowId, minimizedChatWindows, "minimizedChatWindowId");
      RenderSpecificChatWindow(chatCtx);
    });
  }

  function RenderSpecificChatWindow(chatCtx) {
    $("#chatContainer").append(`<div class='chat-window' id='${chatCtx.chatWindowId}'>
      <div class='top'>
        <img src='${chatCtx.profilePicture}' id='${chatCtx.pfpId}'>
        <label id='${chatCtx.nameTagId}'>${chatCtx.firstName} ${chatCtx.lastName}</label>
        <div><img src='/icons/delete.png' id='${chatCtx.minimizeBtnId}'></div>
        <div><img src='/icons/cancel.png' id='${chatCtx.closeBtnId}'></div></div>
      <div class='mid' id='${chatCtx.midId}'>
        <ol id='${chatCtx.contentListId}'></div>
      <div class='bot' id='${chatCtx.botId}'>
        <input placeholder='message...' id='${chatCtx.messageInputId}'><button
        id='${chatCtx.sendBtnId}' class='btn'>Send`);

    chatWindows.push(chatCtx);
    $(`#${chatCtx.minimizeBtnId}`).on("click", () => { MinimizeChatWindow(chatCtx) });
    $(`#${chatCtx.sendBtnId}`).on("click", () => { SendBtn(chatCtx) });
    $(`#${chatCtx.closeBtnId}`).on("click", () => {
      ToggledChatWindow(chatCtx.chatWindowId, chatWindows, "chatWindowId");
    });

    tempCtx = chatCtx;
    Messenger.send("LoadMessages", chatCtx.id);
  }

  // Start connection
  function Success() { console.log("success") }
  function Failure() { console.log("failure") }
  Messenger.start().then(Success, Failure);

});
