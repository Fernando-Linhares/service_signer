export default class Signer
{
    response = {};

    request = {};

    password = {};

    connect() {
        return new Promise((resolve,reject) => {
            try
            {
                this._ws = new WebSocket("ws://localhost:2514/");

                this._ws.onopen = (event) => resolve(event.data);

            } catch (error) {
                reject(error)
            }
        });
    }

    listCertificates() {
        return new Promise((resolve, reject) => {
            try
            {
                this.request = this._ws.send(JSON.stringify({
                    Certificates: {
                        Index: {}
                    }
                }))

                this._ws.onmessage = (event) => resolve(JSON.parse(event.data));

            } catch (error) {
                reject(error)
            }
        });
    }

    sign()
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