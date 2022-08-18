const nameTag = document.getElementById("nameTag");
const profilePicture = document.getElementById("profilePicture");
const searchBox = document.getElementById("searchBox");
const contentList = document.getElementById("contentList");
const messageInput = document.getElementById("messageInput");
const sendBtn = document.getElementById("sendBtn");
let coworkerId = "";

// Create connection
var Messenger = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Hub methods call these
Messenger.on("UsersFiltered", filteredUsers => {
  contentList.innerHTML = "";
  RenderUsers(filteredUsers);
});

Messenger.on("MessagesLoaded", messages => {
  contentList.innerHTML = "";
  messages.forEach(obj => {
    RenderMessage(obj);
  });
  contentList.scrollTop = contentList.scrollHeight;
});

Messenger.on("MessageSent", msg => {
  messageInput.value = "";
  RenderMessage(msg);
  contentList.scrollTop = contentList.scrollHeight;
});

// Call hub methods
searchBox.addEventListener("input", () => {
  Messenger.send("FilterUsers", searchBox.value);
});

function SelectCoworker(evt) {
  coworkerId = evt.currentTarget.coworker.id;
  SwitchHeaderFromSearchToChat(evt.currentTarget.coworker);
  Messenger.send("LoadMessages", coworkerId);
}

sendBtn.addEventListener("click", () => {
  Messenger.send("SendMessage", coworkerId, messageInput.value);
});

// Start connection
function Success() { console.log("success") }
function Failure() { console.log("failure") }
Messenger.start().then(Success, Failure);

// Functions
function SwitchHeaderFromSearchToChat(obj) {
  searchBox.value = "";
  searchBox.setAttribute("placeholder", "");
  searchBox.classname = "disable";

  nameTag.innerHTML = `${obj.firstName} ${obj.lastName}`;
  profilePicture.setAttribute("src", `${obj.profilePicture}`);
  profilePicture.classList.remove("hide");
}


function RenderMessage(msg) {
  const newLi = document.createElement("li");
  const container = document.createElement("div");
  const text = document.createElement("p");
  newLi.className = "conversation";

  if (msg.from === coworkerId) {
    const imgDiv = document.createElement("div");
    const img = document.createElement("img");

    container.className = "msg-in";
    img.setAttribute("src", profilePicture.getAttribute("src"));

    imgDiv.append(img);
    container.append(imgDiv);
  }
  else {
    container.className = "msg-out"
  }

  text.innerHTML = msg.body;
  container.append(text);
  newLi.append(container);
  contentList.append(newLi);
}


function RenderUsers(filteredUsers) {
  if (filteredUsers.length === 0) {
    const newLi = document.createElement("li");
    newLi.innerHTML = "No team members to display...";
    contentList.append(newLi);
  }
  else {
    filteredUsers.forEach(obj => {
      const newLi = document.createElement("li");
      const container = document.createElement("div");
      const img = document.createElement("img");
      const text = document.createElement("p");

      newLi.className = "search-result";
      newLi.addEventListener("click", SelectCoworker);
      newLi.coworker = obj;
      img.setAttribute("src", `${obj.profilePicture}`);
      text.innerHTML = `${obj.firstName} ${obj.lastName}<br/>
        &nbsp;&nbsp;&nbsp;-${obj.assignedRole}`;

      container.append(img);
      container.append(text);
      newLi.append(container);
      contentList.append(newLi);
    });
  }
}
