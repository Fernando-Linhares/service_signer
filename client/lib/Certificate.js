export default class Certificate
{
    id;

    name;

    issuer;

    location_path;

    subject;

    thumbprint;

    constructor(data)
    {
        this.id = data.Id;
        this.issuer = data.Issuer
        this.location = data.Location
        this.name = data.Name
        this.subject = data.Subject
        this.thumbprint = data.Thumbprint
    }
}