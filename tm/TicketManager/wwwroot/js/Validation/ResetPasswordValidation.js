$(document).ready(function () {
  const hide = "hide";
  const valid = "valid";
  const invalid = "invalid";
  let errorExists = false;
  const SpecialChars = ["@", "_", "-", ".", "!", "#", "$", "%", "&", "*", "?"]
  const NumericalChars = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
  const AlphabeticalChars = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]
  const CapitalLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"]

  function ValidChar(char, charList) {
    for (let i = 0; i < charList.length; i++) {
      if (char === charList[i]) { return true }
    }
    return false;
  }

  function ValidEmail(email) {
    let atSign = 0; //The index where @ is present
    let dot = 0; //The index where . is present

    email = email.toLowerCase();
    for (let i = 0; i < email.length; i++) { //Check email prefix
      if (email[i] === "@") { atSign = i; break }

      if (!ValidChar(email[i], AlphabeticalChars) && !ValidChar(email[i], NumericalChars)
        && !ValidChar(email[i], SpecialChars)) { return false }

      if (ValidChar(email[i], SpecialChars)) {
        if (i === 0) { return false } //Special chars can't be first
        if (ValidChar(email[i + 1], SpecialChars)) { return false } //Special chars can't be consecutive
      }
    }
    if (atSign === 0) { return false } //@ Must be present and not first
    if (ValidChar(email[atSign - 1], SpecialChars)) { return false } //Special chars can't be right before @


    for (let j = email.length - 1; j > atSign; j--) { //Check top-level domain
      if (email[j] === ".") {
        if (j > email.length - 3) { return false } //Dot must have at least 2 alphabetical chars after it
        dot = j; break;
      }
      if (!ValidChar(email[j], AlphabeticalChars)) { return false }
    }
    if (dot === 0) { return false } //Dot must be present
    if (dot === atSign + 1) { return false } //There must be a domain between @ and dot


    for (let k = dot - 1; k > atSign; k--) { //Check email domain
      if (!ValidChar(email[k], AlphabeticalChars) && !ValidChar(email[k], NumericalChars)
        && !ValidChar(email[k], SpecialChars)) { return false }
      if (email[k] === "@") { return false } //There can only be one @

      if (ValidChar(email[k], SpecialChars)) {
        if (k === atSign + 1) { return false } //Special chars can't be right after @
        if (k === dot - 1) { return false } //Special chars can't be right before dot
        if (ValidChar(email[k - 1], SpecialChars)) { return false } //Special chars can't be consecutive
      }
    }

    return true;
  }

  $("#emailInput").on("input", () => {
    if (ValidEmail($("#emailInput")[0].value) || $("#emailInput")[0].value === "") {
      HideError("#emailInput", "#errEmail");
    }
    else {
      ShowError("#emailInput", "#errEmail");
    }
  });

  $("#passwordInput").on("input", () => {
    ValidPassword($("#passwordInput")[0].value);
    if (!$(".invalid")[0]) {
      $("#passwordInput").removeClass("err-input");
    }
  });

  function ValidPassword(password) {
    let result = (password.length < 8) ? invalid : valid;
    $("#charMinConfirmation")[0].className = result;
    $("#charLowerConfirmation")[0].className = ContainsChartypeResult(password, AlphabeticalChars);
    $("#charUpperConfirmation")[0].className = ContainsChartypeResult(password, CapitalLetters);
    $("#charNumberConfirmation")[0].className = ContainsChartypeResult(password, NumericalChars);
  }

  function ContainsChartypeResult(passText, chartype) {
    for (let i = 0; i < passText.length; i++) {
      for (let j = 0; j < chartype.length; j++) {
        if (chartype[j] === passText[i]) { return valid }
      }
    }
    errorExists = true;
    return invalid;
  }

  $("#confirmPasswordInput").on("input", () => {
    if ($("#confirmPasswordInput")[0].value !== $("#passwordInput")[0].value) {
      ShowError("#confirmPasswordInput", "#errConfirmPassword");
    }
    else {
      HideError("#confirmPasswordInput", "#errConfirmPassword");
    }
  });

  function ShowError(inputId, errorId) {
    $(inputId).addClass("err-input");
    $(errorId).removeClass(hide);
    $(inputId).on("mouseover", () => {
      $(errorId).removeClass(hide);
    }).on("mouseout", () => {
      $(errorId).addClass(hide);
    });
    errorExists = true;
  }

  function HideError(inputId, errorId) {
    $(inputId).removeClass("err-input");
    $(inputId).off("mouseover");
    $(inputId).off("mouseout");
    $(errorId).addClass(hide);
  }

  $("#resetPasswordForm").on("submit", (evt) => {
    errorExists = false;
    if (!ValidEmail($("#emailInput")[0].value)) {
      ShowError("#emailInput", "#errEmail");
    }
    ValidPassword($("#passwordInput")[0].value);
    if ($(".invalid")[0]) {
      $("#passwordInput").addClass("err-input");
    }
    if ($("#confirmPasswordInput")[0].value !== $("#passwordInput")[0].value) {
      ShowError("#confirmPasswordInput", "#errConfirmPassword");
    }
    if (errorExists) { evt.preventDefault() }
  });
});
