import File from "./File.js";

export default class Modal
{
    content = null;

    setup = {
        enabled_add_files_pdf: true,
        enabled_add_files_pfx: true,
    }

    pdfFile = [];

    certificate = '';

    list_certificates = [];

    constructor(config)
    {
        let {setup, content, pdfFile, list_certificates, certificate} = config;

        if(setup)
            this.setup = setup;

        if(content)
            this.content = content;

        if(pdfFile)
            this.pdfFile = pdfFile;

        if(list_certificates)
            this.list_certificates = list_certificates;

        if(certificate)
            this.certificate = certificate;
    }

    show()
    {
        if(!this.content) {

            let md = this.defaultContent();

            document.body.appendChild(md);

            const background = document.querySelector('.bg-modal-s');

            const buttonCertificate = document.querySelector('#btn-certificate');

            const buttonPdf = document.querySelector('#btn-pdf-file');
            
            const buttonSign = document.querySelector('#save');

            buttonCertificate.addEventListener('click', this.showMiniatureModalAllCertificates.bind(this));

            buttonPdf.addEventListener('click', this.showMiniatureModalAllPdfFiles.bind(this));

            buttonSign.addEventListener('click', this.signFilesdWithCurrentCertificate.bind(this));

            background.addEventListener('click', (event) => {
                if(event.target.classList.contains('bg-modal-s'))
                    this.content.style.display = 'none'
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
                id="bg-progress-bar"
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
                id="bd-progress-bar"
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
                <button
                    id="download-signed"
                    style="
                        cursor: pointer;
                        display: none;
                        border-radius: 4px;
                        border: solid 1px white;
                        width: 30%;
                        height: 10%;
                        position: relative;
                        top: 50%;
                        margin: auto;
                        text-align: center;
                        color: white;
                        background: rgba(200, 0, 0, 0.6);
                        box-shadow: none;
                    "
                >
                    Download
                </button>
            </div>

        </div>`;

        return html;
    }

    showMiniatureModalAllPdfFiles(event)
    {
        if(this.pdfFile.length == 0 && this.setup.enabled_add_files_pdf){

            this.showModalAddPdfFile();

            return;
        }

        let listageFiles = '<div>';


        for(let file of this.pdfFile) {
            listageFiles += `<div
                class="pdf-splited-button"
                style="
                    cursor: pointer;
                    border: solid 1px rgba(0,0,0,0.2);
                    margin: 8px;
                "
            >${file.name}</div>`;
        }

        listageFiles += '</div>';

        let modal = document.createElement('div');

        modal.innerHTML = `<div
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
                    <div style="padding: 10px;  width: 100%; height: 100%;">
                        <div
                            style="
                                display: flex;
                                transition: all 1s;
                                width: 100%;
                                height: 100%;
                            "
                        >
                            <div
                                style="
                                    flex: 1;
                                    transition: all 1s;
                                    border: solid 1px rgba(0,0,0,0.2);
                                    heigh: 100%;
                                    overflow: scroll;
                                "
                            >
                                ${listageFiles}
                            </div>
                            <div style="flex: 3">
                                <div id="image-pdf"
                                    style="
                                        flex: 3;
                                        background: rgba(0, 0, 0, 0.4);
                                        width: 90%;
                                        height: 100%;
                                        margin-left: 30px;
                                    "
                                >

                                </div>
                                <div id="image-pdf-reference"
                                    style="
                                        display: none;
                                        flex: 3;
                                        background: rgba(0, 0, 0, 0.4);
                                        width: 90%;
                                        height: 100%;
                                        margin-left: 30px;
                                    "
                                ></div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>`


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
            bd.style.width = '70%';
            bd.style.height = '70%';
            bd.style.padding = '10px';
        }, 100);

        let pdfSplitedElements = document.querySelectorAll('.pdf-splited-button');

        for(let filepdfelement of pdfSplitedElements) {
            filepdfelement.addEventListener('click', (event) => {

                let el = event.target;

                let currentFile = this.pdfFile.filter(f => f.name == el.textContent);

                if(currentFile.length > 0) {
                    let blob = currentFile[0].toBlob();
                    let url = URL.createObjectURL(blob);
                    let iframe = document.createElement('iframe');
                    iframe.style.width = '100%';
                    iframe.style.height = '100%';
                    iframe.src = url;
                    let container = document.querySelector('#image-pdf');
                    let reference = document.querySelector('#image-pdf-reference');
                    container.innerHTML = '';
                    container.appendChild(iframe);
                    container.style.display = 'block';
                    reference.style.display = 'none';
                }
            });
        }
    }

    showMiniatureModalAllCertificates(event)
    {
        let modal = document.createElement('div');

        modal.innerHTML = '';

        let list_certificates = this.list_certificates;

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

        let sanitize = (name) => {
            if(name.length > 20)
                return name.substring(0, 20) + '...';

            return name;
        };

        radios.forEach(radio => radio.addEventListener('change', (event) =>{
            
            this.certificate = event.target.value;

            let certificateBtn = document.querySelector('#btn-certificate');

            certificateBtn.innerText = sanitize(this.certificate);
            certificateBtn.title = this.certificate;
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
            <input type="file" name="filepdf" id="filepdf" multiple 
                style="
                    display: none;
                "
            >
            <label for="filepdf" id="label-input-file-pdf"
                style="
                    background: rgba(0,0,200,0.6);
                    border-radius: 5px;
                    padding: 10px;
                    color:white;
                    position: relative;
                    top: 20px;
                "
            >
                Select Files
            </label>
            <hr>
            <div
                id="file-updated-image-reference"
                style="
                    display: none;
                    background: rgba(0,0,0,0.3);
                    border-radius: 4px;
                    padding 30px;
                    height: 100px;
                    width: 80%;
                    text-align: center;
                    margin: auto;
                "
            ></div>
            <div id="file-updated-image" style="display: none;"></div>
            <button
                id="save-file-pdf"
                style="
                    background: rgba(0,0,200,0.6);
                    color: white;
                    border: none;
                    border-radius: 5px;
                    padding: 10px;
                    position: relative;
                    top: 8px;
                    "
            >
                Save
            </button>
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

        let inputFile = document.querySelector('#filepdf');

        btnSave.addEventListener('click', (event) => {
            let btnPdfFile = document.querySelector('#btn-pdf-file');

            let files = inputFile.files;

            if(files.length > 0) {

                this.getFiles(inputFile);
    
                if(files.length > 1){
                    let nameInput = sanitize(files[0].name) + ' (+'+(files.length - 1)+')';
                    btnPdfFile.innerText = nameInput;
                    btnPdfFile.title = files[0].name +  ' (+'+(files.length - 1)+')';
                }

                if(files.length == 1){
                    let nameInput = sanitize(files[0].name);
                    btnPdfFile.innerText = nameInput;
                    btnPdfFile.title = files[0].name;
                }

                modal.style.display = 'none';

                modal.remove();
            }
        });

        let bd = document.querySelector('.bd-add-file-modal-s');

        let imageContainerReference = document.querySelector('#file-updated-image-reference');

        setTimeout(() => {
            bd.style.width = '30%';
        },100);

        let sanitize = (name) => {
            if(name.length > 20)
                return name.substring(0, 20) + '...';

            return name;
        };

        imageContainerReference.style.display = 'block';

        let labelInputFile = document.querySelector('#label-input-file-pdf');

        inputFile.addEventListener('change', (event) => {
           
           let imageContainer = document.querySelector('#file-updated-image');

           imageContainer.style.display = 'block'

           let files = event.target.files;

           imageContainerReference.style.display = 'none';

           if(files.length > 1) {
               let nameInput = sanitize(files[0].name) + ' (+'+(files.length - 1)+')';

               labelInputFile.innerText = nameInput;

               labelInputFile.title = files[0].name;
           }

           if(files.length == 1) {
                let nameInput = sanitize(files[0].name);

                labelInputFile.innerText = nameInput;;

                labelInputFile.title = files[0].name;
           }

            for(let file of files) {
                let iframe = document.createElement('iframe');
                let blob = new Blob([file], { type: "application/pdf" });
                let url = URL.createObjectURL(blob);
                iframe.src = url;
                imageContainer.appendChild(iframe);
            }
        });
    }

    async signFilesdWithCurrentCertificate()
    {
        if(this.certificate && (this.pdfFile.length > 0)) {

            const buttonSign = document.querySelector('#save');

            const background =  document.querySelector('#bg-progress-bar');

            const body = document.querySelector('#bd-progress-bar');

            const buttonDownlod = document.querySelector('#download-signed');

            buttonSign.style.display = 'none';

            background.style.display = 'block';

            for(let i = 0; i <= this.pdfFile.length; i++) {

                let percent = ( i * 100 ) / this.pdfFile.length;
                
                if(i == this.pdfFile.length){
                    body.style.width = percent + '%';
                    break;
                }

                // let response = await this.requestSignature(this.pdfFile[i], this.certificate);

                body.style.width = percent + '%';
            }

            background.style.display = 'none';

            buttonDownlod.style.display = 'block';
        }
    }

    async requestSignature(file, cert)
    {

    }

}