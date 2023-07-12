export default class File
{
    name = null;

    content = null;

    constructor(name, content)
    {
        this.name = name;

        this.content = content;
    }

    toString()
    {
        return this.content;
    }

    toBlob()
    {
        let content = atob(this.content);

        let byteArray = new Uint8Array(content.length);

        for(let i = 0; i < content.length; i++) {
            byteArray[i] = content.charCodeAt(i);
        }

        let blob = new Blob([byteArray], {type: 'application/pdf'});

        blob.name = this.filename;

        return blob;
    }
}