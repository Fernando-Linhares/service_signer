import File from "./File.js";

export default class Modal
{
    content = null;

    pdfFile = [
        // { name:'ARQUIVO EXEMPLO 1' },
        // { name:'ARQUIVO EXEMPLO 2' },
        // { name:'ARQUIVO EXEMPLO 3' },
        // { name:'ARQUIVO EXEMPLO 4' },
    ];

    certificate = '';

    list_certificates = [
        { name:'CERTIFICADO EXEMPLO 1' },
        { name:'CERTIFICADO EXEMPLO 2' },
        { name:'CERTIFICADO EXEMPLO 3' },
        { name:'CERTIFICADO EXEMPLO 5' },
        { name:'CERTIFICADO EXEMPLO 6' },
        { name:'CERTIFICADO EXEMPLO 7' },
        { name:'CERTIFICADO EXEMPLO 8' },
    ];

    show()
    {
        if(!this.content) {

            let md = this.defaultContent();

            document.body.appendChild(md);

            const background = document.querySelector('.bg-modal-s');

            const buttonCertificate = document.querySelector('#btn-certificate');

            const buttonPdf = document.querySelector('#btn-pdf-file');

            buttonCertificate.addEventListener('click', this.showMiniatureModalAllCertificates.bind(this));

            buttonPdf.addEventListener('click', this.showMiniatureModalAllPdfFiles.bind(this));

            background.addEventListener('click', (event) => {
                if(event.target.classList.contains('bg-modal-s')) {
                    this.content.style.display = 'none'
                }
            }, false);

            this.content = md;
        }

        const body = document.querySelector('.bd-modal-s');

        setTimeout(() => {
            body.style.width = '40%';
            body.style.height = '50%';
        }, 100);

        return this.content.style.display = 'block';
    }

    hide()
    {
        if(!this.content)
            this.content = defaultContent();

        return this.content.style.display = 'none';
    }

    defaultContent()
    {
        let pdfFile = "Select Pdf File"

        if(this.pdfFile.length > 0)
            pdfFile = this.pdfFile[0].name;

        if(this.pdfFile.length > 1)
            pdfFile = this.pdfFile[0].name + ' (+' + (this.pdfFile.length - 1) +')';

        let certificate = "Select Certificate";

        if(this.certificate)
            certificate = this.certificate;

        let html = document.createElement('div');

        html.innerHTML =  `<div
            class="bg-modal-s"
            style="
                display: block;
                position: fixed;
                background: rgba(0,0,0,0.3);
                top: 0;
                bottom: 0;
                right: 0;
                left: 0;
                z-index: 10000;
            ">
            <div
                class="bd-modal-s"
                style="
                    transition:all 0.1s;
                    position:relative;
                    top: 70px;
                    display: fixed;
                    width: 10%;
                    height: 10%;
                    background: white;
                    border-radius: 5px;
                    text-align:center;
                    margin: auto;
                "
            >
            <h2 style="
                    text-align: center;
                    position: relative;
                    top: 30px;
                    font-size: 30px;
                "
            >
                Signature
            </h2>

            <div style="position: relative; top: 40%; display: flex;">
                <div style="flex: 3;">
                    <button
                        id="btn-certificate"
                        style="
                            cursor: pointer;
                            color: white;
                            background: rgba(0, 0, 0, 0.8);
                            box-shadow: none;
                            padding: 7px;
                            border-radius: 4px;
                            border: solid 1px white;
                        "
                    >
                    ${certificate}
                    </button>
                </div>
                <div style="flex: 3;">
                    <button
                        id="btn-pdf-file"
                        style="
                            cursor: pointer;
                            color: white;
                            background: rgba(200, 0, 0, 0.6);
                            box-shadow: none;
                            padding: 7px;
                            border-radius: 4px;
                            border: solid 1px white;
                        "
                    >
                        ${pdfFile}
                    </button>
                </div>
            </div>
            <button
                id="save"
                style="
                    cursor: pointer;
                    display: block;
                    border-radius: 4px;
                    border: solid 1px white;
                    width: 30%;
                    height: 10%;
                    position: relative;
                    top: 50%;
                    margin: auto;
                    text-align: center;
                    color: white;
                    background: rgba(0, 0, 200, 0.6);
                    box-shadow: none;
                "
            >
                Sign
            </button>
            <div
                style="
                    display: none;
                    border-radius: 4px;
                    border: solid 1px rgba(0, 140, 0, 0.8);
                    width: 80%;
                    height: 10%;
                    position: relative;
                    top: 50%;
                    margin: auto;
                    text-align: center;
                "
            > 
            <div 
                style="
                    background: rgba(0, 140, 0, 0.8);
                    width: 80%;
                    height: 100%;
                    position: relative;
                    top: 0;
                    bottom: 0;
                    position: relative;
                    text-align: left;
                    transition: all 1s;
                "
            ></div>
            </div>
            </div>
        </div>`;

        return html;
    }

    showMiniatureModalAllPdfFiles(event)
    {

        if(this.pdfFile.length == 0){

            this.showModalAddPdfFile();

            return;
        }

        let modal = document.createElement('div'); 

        modal.innerHTML = this.buildMiniatureByList(this.pdfFile.map(file => {

            return `
            <div style="padding: 10px;">
                <label>${file.name}</label>
            </div>
            <hr>`;
        }));

        document.body.appendChild(modal);

        const bgModal = document.querySelector('.bg-minature-modal-s');

        bgModal.addEventListener('click', (event) => {
            if(event.target.classList.contains('bg-minature-modal-s')) {
                modal.style.display = 'none';
                modal.remove();
            }
        })

        setTimeout(()=> {
            let bd = document.querySelector('.bd-minature-modal-s');
            bd.style.width = '30%';
            bd.style.padding = '10px';
        }, 100);
    }

    showMiniatureModalAllCertificates(event)
    {
        let modal = document.createElement('div');

        modal.innerHTML = '';

        modal.innerHTML = this.buildMiniatureByList(this.list_certificates.map(cert =>
        {
            let is_checked = cert.name == this.certificate ? 'checked="true"' : '';

            return `
            <div style="padding: 10px;">
                <div style="display: flex;">
                    <div style="flex: 6;">
                        <label>${cert.name}</label>
                    </div>
                    <div style="flex: 1;">
                        <input type="radio" id="cert-radio" value="${cert.name}" name="certificate" ${is_checked}>
                    </div>
                </div>
            </div>
            <hr>`;
        }));

        document.body.appendChild(modal);

        const bgModal = document.querySelector('.bg-minature-modal-s');

        bgModal.addEventListener('click', (event) => {
            if(event.target.classList.contains('bg-minature-modal-s')) {
                modal.style.display = 'none';
                modal.remove();
            }
        });

        const radios = document.querySelectorAll('#cert-radio');

        radios.forEach(radio => radio.addEventListener('change', (event) =>{
            this.certificate = event.target.value;

            document.querySelector('#btn-certificate').innerText = this.certificate;
        }));

        setTimeout(()=> {
            let bd = document.querySelector('.bd-minature-modal-s');
            bd.style.width = '30%';
            bd.style.padding = '10px';
        }, 100);
    }

    buildMiniatureByList(list)
    {
        let content = '<div style="border: solid 1px rgba(0, 0, 130, 0.1)">';
        
        for(let item of list){
            content += `<div>
                ${item}
            </div>`;
        }

        content += '</div>';

        return `
        <div
            class="bg-minature-modal-s"
            style="

                position: fixed;
                top: 0;
                bottom: 0;
                left: 0;
                right: 0;
                z-index: 1000000000;
                background: rgba(0,0,0,0.4);
            "
        >
            <div
                class="bd-minature-modal-s"
                style="
                    text-align: center;
                    margin: auto;
                    transition: all 0.1s;
                    background: white;
                    width: 5%;
                    padding: 1px;
                    position: relative;
                    top: 100px;
                    border-radius: 5px;
                "
            >
                <div class="c-modal"
                    style="
                        overflow: scroll;
                        height: 250px;
                    "
                >
                ${content}
                </div>
            </div>
        </div>
        `;
    }

    convertFileToBase64(file) {
        return new Promise((resolve, reject) => {
          const reader = new FileReader();
      
          reader.onload = () => {
            const base64Content = reader.result.split(',')[1];
            resolve(base64Content);
          };
      
          reader.onerror = (error) => {
            reject(error);
          };
      
          reader.readAsDataURL(file);
        });
      }

    async getFiles(inputFile) {

        for(let file of inputFile.files) {

            let content = await this.convertFileToBase64(file);

            let fileStream = new File(file.name, content);

            this.pdfFile.push(fileStream);
        }
    }

    templateAddFile(content) {

        return `
            <div
            class="bg-add-file-modal-s"
            style="

                position: fixed;
                top: 0;
                bottom: 0;
                left: 0;
                right: 0;
                z-index: 1000000000;
                background: rgba(0,0,0,0.4);
            "
        >
            <div
                class="bd-add-file-modal-s"
                style="
                    text-align: center;
                    margin: auto;
                    transition: all 0.1s;
                    background: white;
                    width: 5%;
                    padding: 1px;
                    position: relative;
                    top: 100px;
                    border-radius: 5px;
                "
            >
                <div class="c-modal"
                    style="
                        overflow: scroll;
                        height: 250px;
                    "
                >
                ${content}
                </div>
            </div>
        </div>
        `;
    }

    showModalAddPdfFile()
    {
        let modal = document.createElement('div');

        let content = `
            <input type="file" name="filepdf" id="filepdf" multiple>
            <hr>
            <br>
            <button id="save-file-pdf">
                Save
            <button>
        `;

        modal.innerHTML = this.templateAddFile(content);

        modal.style.display = 'block';

        document.body.appendChild(modal);

        let bg = document.querySelector('.bg-add-file-modal-s');

        bg.addEventListener('click', (event) => {
            if(event.target.classList.contains('bg-add-file-modal-s')) {

                modal.style.display = 'none';

                modal.remove();
            }
        })

        let btnSave = document.querySelector('#save-file-pdf');

        btnSave.addEventListener('click', (event) => {
            this.getFiles(document.querySelector('#filepdf'));
        
            console.log(this.pdfFile)
        });

    }
}