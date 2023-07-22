export default class Signer
{
    _conn;
    
    _route = "ws://localhost:2514";

    connect() {
        return new Promise((resolve, reject) => {
            try
            {
                let ws = new WebSocket(this._route);

                this._conn = ws;

                ws.onopen = (event) => resolve(event.data);

            } catch (error)
            {
                reject(error)
            }
        });
    }

    listCertificates() {
        return new Promise((resolve, reject) => {
            try
            {
                this._conn.send(JSON.stringify({
                    command: "list.certificates"
                }));

                this._conn.onmessage = (event) => resolve(JSON.parse(event.data));

            } catch (error) {
                reject(error)
            }
        });
    }

    sign(file, certificate, password) {

        return new Promise((resolve, reject) => {
            try
            {
                let request = {
                    command: "sign.file",
                    filename: file.name,
                    filecontent: file.content,
                    password: password,
                    certid: certificate.id
                };
                console.log("ok ");
                let body = JSON.stringify(request);

                this._conn.send(body);

                this._conn.onmessage = (event) => resolve(event.data);
            }
            catch (error)
            {
                this._conn.onerror = (event) => reject(event.data);

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