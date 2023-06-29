var port = 3459;

chrome.runtime.onMessage.addListener(
    (request, sender, sendResponse) => {
        port = connect('com.signer.service');
        console.log(sendResponse)
        // makeXml(request);
        onDisconnect();
    }
);

function connect(host){
    return chrome.runtime.connectNative(host);
}

function onConnect() {
    port.onConnect.addListener(function (request, sender, sendResponse) {
        console.log(sendResponse)
    });
}

function onDisconnect() {
    port.onDisconnect.addListener(function () {
        if (chrome.runtime.lastError) {
            console.log(chrome.runtime.lastError);
        }
    });
}

// function makeXml(request){
//     port.postMessage({ 
//         user: request.user,
//         token: request.public_key,
//         group: request.group,
//         box: request.box,
//         regenerate: request.regenerate
//     });
// }