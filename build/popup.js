// document.querySelector('#btn-lc').addEventListener('click', () => {
//     port = chrome.runtime.connectNative('com.signer.service');


//     onDisconnect();
// })

// function onDisconnect() {
//     port.onDisconnect.addListener(function () {
//         if (chrome.runtime.lastError) {
//             console.log(chrome.runtime.lastError);
//         }
//     });
// }


// on popup of extension 
let signer_options = {};

let buttonSign = document.querySelector('#btn-sgn');

let buttonListCert = document.querySelector('#btn-lc');

if(buttonSign)
    buttonSign.addEventListener('click', sign.bind({options: signer_options}));

if(buttonListCert)
    buttonListCert.addEventListener('click', listCerts.bind({options: signer_options}));

function listCerts(event) {

    if(event.target.classList.contains('actived'))
        return chrome.runtime.sendMessage({ control: 0 });
}

function sign(event) {

    if(!this?.options?.file)
        return console.error("Error ( field \"file\" is not found in signer_options )");

    if(!this?.options?.index)
        return console.error("Error ( field \"index\" is not found in signer_options )");

    if(!this?.options?.password)
        return console.error("Error ( field \"password\" is not found in signer_options )");

    if(this?.options && event.target.classList.contains('actived'))
        return chrome.runtime.sendMessage({
            control: 1,
            form: {
                file: this.options.file,
                index: this.options.index,
                password: this.options.password
            }
        });
}