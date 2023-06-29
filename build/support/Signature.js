/**
 * Ferramenta que da suporte à assinatura de documentos
 * 
 * @class Signature
 * @methods render
 * @getter @setter data
 */

export default class Signature
{
    _data;

    constructor(data)
    {
       this._data = data;
    }

    getData()
    {
        return this._data;
    }

    setData(data)
    {
        this._data = data;
    }

    render()
    {
        let elements = [
            document.createElement('#user'),
            document.createElement('#token'),
            document.createElement('#group'),
            document.createElement('#box'),
        ];

       let data = this.getData();

       elements[0].value = data.user;
       elements[1].value = data.token;
       elements[2].value = data.group;
       elements[3].value = data.box;

       if(data?.hidden){
           elements.forEach(element => {
                element.style.display = 'none'
            })
       }
    }
}