export default class Signer
{
    response = {};

    request = {};

    password = {};

    constructor()
    {
        this._ws = new WebSocket("ws://localhost:2514/");

        this._ws.onmessage = (event) => this.response = event.data;
    }

    save()
    {
        this.request = {
            Signature: {
                Sign: {
                    FileName: this.file.name,
                    FileContent: this.file.content,
                    Password: this.password,
                    CertId: this.certificate.id
                }
            }
        };

        this._ws.send(Json.stringfy(this.request));
    }

    setFile(file)
    {
        this.file = file;
    }

    setCertificate(certificate)
    {
        this.certificate = certificate;
    }

    setPassword()
    {
        this.password = password;
    }

    showModal()
    {
        
    }

    showFile()
    {

    }

    downloadSigned()
    {

    }
}