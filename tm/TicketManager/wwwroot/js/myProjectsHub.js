$(document).ready(function () {
  const hide = "hide";

  var MyProjConnection = new signalR.HubConnectionBuilder().withUrl("/myProjectsHub").build();
  function Success() { console.log("MyProjConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  MyProjConnection.start().then(Success, Failure);
  MyProjConnection.onclose(async () => await MyProjConnection.start());

  MyProjConnection.on("GetProjCardCtx", (projectList, pfpList, projCardCtxList) => {
    (projectList.length === 0) ? $("#noProjMsg").removeClass(hide) : $("#noProjMsg").addClass(hide);
    if (projCardCtxList[0].clearBeforeRendering) {
      $("#projCardDiv")[0].innerHTML = "";
    }
    for (let i = 0; i < projectList.length; i++) {
      RenderProjCard(projectList[i], pfpList[i], projCardCtxList[i]);
    }
  });

  function RenderProjCard(proj, pfpArr, projCardCtx) { //title asp
    $("#projCardDiv").prepend(
      `<article id="${proj.id}">
      <a id="${projCardCtx.titleId}" class="title" href="/Main/${projCardCtx.role}Dashboard">${proj.name}</a>
      <div id="${projCardCtx.topDivId}" class="top">
        <div>
          <img src="/icons/bell.png">
          <span id="${projCardCtx.notiCountId}">${projCardCtx.notificationCount}</span>
        </div>
        <div>
          <img src="/icons/comment.png">
          <span id="${projCardCtx.msgCountId}">${projCardCtx.messageCount}</span>
        </div>
        <div id="${projCardCtx.pfpDivId}" class="pfp-div">
          <span class="user-span">${proj.teamMemberCount}</span>
        </div>
      </div>
      <div class="mid">
          <button tabindex="0" class="btn" id="${projCardCtx.copyBtnId}">Copy</button>
          <input id="${projCardCtx.projectCodeId}" value="${proj.projectCode}" readonly></input>
      </div>
      <div class="bot">
        <button tabindex="0" class="btn" id="${projCardCtx.leaveBtnId}">Leave</button>
        <a class="btn" href="${proj.gitHubLink}">GitHub</a>
        <a id="${projCardCtx.openBtnId}" class="btn" href="/Main/${projCardCtx.role}Dashboard">Open</a>

      </div>
    </article>`);
    for (let i = 0; i < pfpArr.length; i++) {
      const pfpClass = (pfpArr.length === 1) ? "single-pfp" : `pfp${i}`;
      $(`#${projCardCtx.pfpDivId}`).append(`<img class="${pfpClass}" src="${pfpArr[i]}">`);
    }
    $(`#${proj.id}`).addClass("article-full");
    if (projCardCtx.messageCount === 0) {
      $(`#${projCardCtx.msgCountId}`).addClass(hide);
    }
    if (projCardCtx.notificationCount === 0) {
      $(`#${projCardCtx.notiCountId}`).addClass(hide);
    }
    $(`#${projCardCtx.copyBtnId}`).on("click", () => {
      $(`${projCardCtx.projectCodeId}`).select();
      navigator.clipboard.writeText(`${proj.projectCode}`.replace(/ /g, ""));
    });
    if (projCardCtx.projCodeResult === "newProj") {
      $(`#${projCardCtx.topDivId}`)[0].innerHTML = "";
      $(`#${projCardCtx.topDivId}`).append("<p>pending approval...</p>");
      $(`#${projCardCtx.titleId}`).addClass("disable");
      $(`#${projCardCtx.openBtnId}`).addClass("disable");
    }
    $(`#${projCardCtx.leaveBtnId}`).on("click", () => {
      $("#deleteConfirmationModal").removeClass(hide);
      $("#myProjMain").addClass("screen-tint");
      $("#yesDelBtnWrap").append(`<button tabindex="0" class="btn" id="${projCardCtx.yesDelBtn}">Leave</button>`);
      $(`#${projCardCtx.yesDelBtn}`).on("click", () => {
        $("#deleteConfirmationModal").addClass(hide);
        $("#myProjMain").removeClass("screen-tint");
        $("#yesDelBtnWrap")[0].innerHTML = "";
        MyProjConnection.send("LeaveProject", proj.id);
      });
    });
    $(`#${projCardCtx.openBtnId}`).on("click", () => {
      MyProjConnection.send("OpenProject", proj.id);
    });
    $(`#${projCardCtx.titleId}`).on("click", () => {
      MyProjConnection.send("OpenProject", proj.id);
    });
  }

  MyProjConnection.on("JoinProject", (proj, pfpArr, projCardCtx) => {
    $("#projCodeTxt")[0].value = "";
    $("#noProjMsg").addClass(hide);
    RenderProjCard(proj, pfpArr, projCardCtx);
  });
  MyProjConnection.on("JoinProjErr", result => {
    DisplayProjectError(result);
  });
  MyProjConnection.on("DeleteProjCard", projectId => {
    $(`#${projectId}`)[0].remove();
    MyProjConnection.send("LoadProjects");
  });
  
  function DisplayProjectError(result) {
    switch (result) {
      case "invalid": $("#invalidCodeErr").removeClass(hide); break;
      case "repeat": $("#repeatCodeErr").removeClass(hide); break;
    }
    $("#projCodeTxt").addClass("err-input");
  }

  $("#projCodeTxt").on("input", () => {
    $("#invalidCodeErr").addClass(hide);
    $("#repeatCodeErr").addClass(hide);
    $("#projCodeTxt").removeClass("err-input");
  });

  function PageLoad() {
    $("#myProjMain").removeClass(hide);
    MyProjConnection.send("LoadProjects");
  }

  $("#projSearch").on("input", () => {
    const filterString = $("#projSearch").val();
    MyProjConnection.send("SearchProject", filterString);
  });
  $("#openCreateProjBtn").on('click', () => {
    $("#newProjModal").removeClass(hide);
    $("#myProjMain").addClass("screen-tint");
  });
  $("#createBtn").on('click', () => {
    $("#newProjModal").addClass(hide);
    $("#myProjMain").removeClass("screen-tint");
    const formData = {
      name: $("#projTitleTxt").val(),
      gitHubLink: $("#gitHubLinkTxt").val(),
      startDate: $("#startDate").val(),
      endDate: $("#endDate").val()
    }
    $("input").val("");
    MyProjConnection.send("CreateProject", formData);
  });
  $("#closeBtn").on('click', () => {
    $("#newProjModal").addClass(hide);
    $("#myProjMain").removeClass("screen-tint");
  });
  $("#joinProjBtn").on("click", () => {
    let projectCode = $("#projCodeTxt")[0].value.toUpperCase();
    MyProjConnection.send("JoinProject", projectCode);
  });
  $("#noDelBtn").on("click", () => {
    $("#deleteConfirmationModal").addClass(hide);
    $("#myProjMain").removeClass("screen-tint");
    $("#yesDelBtnWrap")[0].innerHTML = "";
  });
});
