$(document).ready(function () {
  var AdminDashConnection = new signalR.HubConnectionBuilder().withUrl("/adminDashHub").build();
  function Success() { console.log("AdminDashConnection success"); PageLoad() }
  function Failure() { console.log("failure") }
  AdminDashConnection.start().then(Success, Failure);
  AdminDashConnection.onclose(async () => await AdminDashConnection.start());

  function PageLoad() {
    AdminDashConnection.send("LoadTeamMembers");
  }

  AdminDashConnection.on("GetTeamMembers", memberCtxList => {
    memberCtxList.forEach(ctx => {
      if (ctx.isApproved) {
        RenderTeamMember(ctx);
      }
      else {
        RenderPendingMember(ctx);
      }
    });
  });

  AdminDashConnection.on("AddTeamMember", (ctx) => {
      RenderTeamMember(ctx);
  });
  
  function RenderPendingMember(ctx) {
    $("#newUserDiv").append(`
        <article id="${ctx.appUserId}">
          <div class="name-div">
            <img src="${ctx.pfp}">
            <span>${ctx.firstName} ${ctx.lastName}<br>&nbsp; -${ctx.role}</span>
          </div>
          <div class="options-div">
            <div id="${ctx.approveBtnId}" class="confirm"><div></div><div></div></div>
            <div id="${ctx.denyBtnId}" class="invalid"><div></div><div></div></div>
          </div>
        </article>
    `);
    $(`#${ctx.approveBtnId}`).on("click", () => {
      $(`#${ctx.appUserId}`)[0].remove();
      AdminDashConnection.send("ApprovePendingMember", ctx.appUserId);
    });
    $(`#${ctx.denyBtnId}`).on("click", () => {
      $(`#${ctx.appUserId}`)[0].remove();
      AdminDashConnection.send("RemoveMember", ctx.appUserId);
    });
  }

  function RenderTeamMember(ctx) {
    $("#teamDiv").append(`
        <article id="${ctx.appUserId}">
          <div class="name-div">
            <img src="${ctx.pfp}">
            <span>${ctx.firstName} ${ctx.lastName}<br>&nbsp; -${ctx.role}</span>
          </div>
          <div class="options-div">
            <button id="${ctx.removeBtnId}" class="btn" tabindex="0">remove</button>
          </div>
        </article>`);
    $(`#${ctx.removeBtnId}`).on("click", () => {
      $(`#${ctx.appUserId}`)[0].remove();
      AdminDashConnection.send("RemoveMember", ctx.appUserId);
    });
  }
});
