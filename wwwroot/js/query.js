function toggleButtonState(buttonId, isSubmitting) {
    var submitButton = document.getElementById(buttonId);
    if (!submitButton) return;

    var spinner = submitButton.querySelector('.spinner-border');
    var buttonText = submitButton.querySelector('.button-text');

    if (isSubmitting) {
        submitButton.disabled = true;
        spinner.style.display = 'inline-block'; // Show spinner
        buttonText.style.display = 'none'; // Hide text
    } else {
        submitButton.disabled = false;
        spinner.style.display = 'none'; // Hide spinner
        buttonText.style.display = 'inline-block'; // Show text (magnifying glass icon)
    }
}

function disableInputField(inputId, isDisabled) {
    var inputField = document.getElementById(inputId);
    if (!inputField) return;
    inputField.disabled = isDisabled;
}

document.addEventListener('DOMContentLoaded', function () {
    var spinners = document.querySelectorAll('.spinner-border');
    spinners.forEach(function (spinner) {
        spinner.style.display = 'none'; // Hide all spinners by default
    });
});