/**
        Os campos necessários para assinatura precisam possuir os is

        group : para grupo de assinatura
        user : para o email do usuario assinador
        box : para caixa em que os arquivos pode ser encontrados
        token : para a chave públic a ser utilizada

    -----------------------------------------------------------

    Observações.

        * token (chave pública) - 

          Pode ser gerado tanto no aplicativo do assinador em 
        configurações > chave publica > gerar chave >
        (Fazer login utilizando o usuario registrado na api e chave privada).
            A chave pública também pode ser gerada fazendo login na api com 
        chave privada.

        * group (grupo de assinaturas) -

            Corresponde a um conjunto de assinaturas desiginadas para determinado
         usuário. Pode ser criado na api utilizando utilizando os arquivos a serem
         assinados e o email do usuário


         * user (usuário assinador) -

            Geralmente o usuario designado para assinar um determinado documento ou
        vários, podendo ser definas asssinaturas na api.

        * caixa (caixa de arquivos) -

            Essa é a chave que determina onde os arquivos estão localizados na api
        para serem devidamente tratados.

**/

function request(body){
    chrome.runtime.sendMessage(body)
}

function groupName(){
    return value("group");
}

function boxName(){
    return value("box");
}

function publicKey()
{
    return value("token");
}

function userEmail(){
    return value("user");
}

function value(id){
    return document.querySelector("#" + id).value;
}

if(document.querySelector('#button-signer.active')){
    document.querySelector('#button-signer.active').addEventListener('click', () => {
        request({
            user: userEmail(),
            group: groupName(),
            box: boxName(),
            public_key: publicKey(),
            regenerate: "true"
        });
    });
}

// chrome.runtime.sendMessage('com.signer.service', {list:''}, function(res){
//     if (chrome.runtime.lastError) {
//         console.error(chrome.runtime.lastError);
//         return;
//       }
      
//       // Process the response from the native application
//       console.log('Received response:', response);
// });