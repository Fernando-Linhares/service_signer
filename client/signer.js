import Signer from './lib/Signer.js';
import Modal from './lib/Modal.js';
import Certificate from './lib/Certificate.js';

async function signer(setup) {

    let signator = new Signer();

    await signator.connect();

    let responseSigner = await signator.listCertificates();

    let id = 1;

    setup.list_certificates = responseSigner
    .certificates
    .map(certData => {

        certData.Id = id;

        id++;

        return new Certificate(certData);
    })
    .filter(cert => cert.name !== 'localhost')

    setup.signatureRequest = async (file, certificate, password) => await signator.sign(file, certificate, password);
 
    let modal = new Modal(setup); 

    setup.button.addEventListener('click', executeSignature.bind({ modal, signator }));
}

async function executeSignature(event) {
    if(!event.target.classList.contains('disabled')){
        this.modal.show();
    }
}

export default signer;