let file = null;

async function init() {
   const response = await fetch('http://localhost:8080/ajax.php', { method:'GET' }).then(res => res.json());

   file = file_pdf('myfile000.pdf', response.data);
}

init();

let btnSign = document.querySelector('#btn-lc');

btnSign.addEventListener('click', signFile);

function signFile(event) {

    if(file) {

        const client = signer();

        client.connect();

        console.log(client.state)

        // client.sign({
        //     Signatures: {
        //         Sign: {
        //             CertId:"1",
        //             Password:"1234",
        //             FileName: file.filename,
        //             FileContent: file.toString()
        //         }
        //     }
        // });
    }
}
