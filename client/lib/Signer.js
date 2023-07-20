export default class Signer
{
    _conn;

    connect(route) {
        return new Promise((resolve, reject) => {
            try
            {
                let ws = new WebSocket(route);

                this._conn = ws;

                ws.onopen = (event) => resolve(this);

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
                this._conn.send(JSON.stringify({}));

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
                let request = {
                    FileName: file.name,
                    FileContent: file.content,
                    Password: password,
                    CertId: certificate.id
                };

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