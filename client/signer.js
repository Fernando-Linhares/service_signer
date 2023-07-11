function config() {

    const host = 'localhost';

    const protocolo = 'ws';

    const port = '2514';

    const websocket = { protocolo,  host, port };

    return { websocket };
}

function signer() {

    const instance = {};

    const state = {};

    const certificate_instance = certificate.bind({ state });

    instance.connect = connect_websocket_server.bind({ state });

    instance.disconect = disconect_websocket_server.bind({ state });

    instance.state = state;

    instance.is_connected = state?.is_connected;

    instance.sign = sign_pdf.bind({ state });

    // instance.signAl  l = sign_all_pdf.bind({ state });

    instance.certificate = certificate_instance();

    return instance;
}

// function sign_all_pdf(body) {
//     if(this.state?.is_connected)
//         for(var file of file_pdf_list)
//             sign_pdf(body);
// }

function sign_pdf(body) {

    this.state.ws.send(JSON.stringify(body));
}

function certificate() {

    let instance = {};

    instance.add = certificate_add;
    instance.remove = certificate_remove;
    instance.list = certificate_list;
    instance.update = certificate_update;

    return instance;
}

function certificate_add() {
    
}

function certificate_remove() {

}

function certificate_list() {

}

function certificate_update() {

}

async function download(url) {

}

async function post(form) {

}

function server(route) {

    return new WebSocket(route);
}

function connect_websocket_server() {

    let state = this.state;

    let data = config();

    let { port, protocolo, host } = data.websocket;

    let ws = server(`${protocolo}://${host}:${port}/`);

    ws.addEventListener('open', onConnect.bind({ state }));

    ws.addEventListener('close', onClose.bind({ state }));

    ws.addEventListener('error', onError.bind({ state }));

    ws.addEventListener('message', onMessage.bind({ state }))

    this.state.ws = ws;
}

function disconect_websocket_server()
{

}

function onConnect(event) {

}

function onClose(event) {

}

function onError(event) {

}

function onMessage(event) {

}

function file_pdf(filename, content)
{
    const instance = {};

    instance.filename = filename;

    instance.content = content;

    instance.toBlob = convertToBlob.bind({file:{instance}});

    instance.toString = () => instance.content;

    return instance;
}

function convertToBlob()
{
    let data = this.file;

    const blob =  new Blob([data.content], { type: 'application/pdf' });

    blob.name = data.filename;

    return blob;
}