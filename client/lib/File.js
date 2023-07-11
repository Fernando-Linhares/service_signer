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
        const blob =  new Blob([this.content], { type: 'application/pdf' });

        blob.name = this.filename;

        return blob;
    }
}