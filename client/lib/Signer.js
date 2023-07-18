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

    sign(file, certificate, password) {

        return new Promise((resolve, reject) => {
            try
            {
                this.request = {
                    Signatures: {
                        Sign: {
                            FileName: file.name,
                            FileContent: file.content,
                            Password: password,
                            CertId: certificate.id
                        }
                    }
                };

                let body = JSON.stringify(this.request);
    
                this._ws.send(body);

                this._ws.onerror = (event) => reject(event.data)

                this._ws.onmessage = (event) => resolve(event.data);
            }
            catch (error)
            {
                reject(error);
            }
        });
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