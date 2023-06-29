
var port = 3459;

document.querySelector('#signer').addEventListener('click', () => {
    port = chrome.runtime.connectNative('com.signer.service');
    onDisconnect();
})

function onDisconnect() {
    port.onDisconnect.addListener(function () {
        if (chrome.runtime.lastError) {
            console.log(chrome.runtime.lastError);
        }
    });
}
