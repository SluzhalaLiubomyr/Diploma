document.addEventListener('DOMContentLoaded', function () {
    chrome.tabs.query({ active: true, currentWindow: true }, function (tabs) {
        const url = tabs[0].url;
        if (!url) {
            document.getElementById('result').innerText = "Unable to get URL.";
            return;
        }
    });
});