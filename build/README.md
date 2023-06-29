### Os campos necessários para assinatura

group : para grupo de assinatura
user : para o email do usuario assinador
box : para caixa em que os arquivos pode ser encontrados
token : para a chave pública a ser utilizada

-----------------------------------------------------------

### Observações.

#### token (chave pública) -

Pode ser gerado tanto no aplicativo do assinador em 
configurações > chave publica > gerar chave >
(Fazer login utilizando o usuario registrado na api e chave privada).
A chave pública também pode ser gerada fazendo login na api com 
chave privada.

#### group (grupo de assinaturas) -

Corresponde a um conjunto de assinaturas desiginadas para determinado
usuário. Pode ser criado na api utilizando utilizando os arquivos a serem
assinados e o email do usuário


#### user (usuário assinador) -

Geralmente o usuario designado para assinar um determinado documento ou
vários, podendo ser definas asssinaturas na api.

#### caixa (caixa de arquivos) -

Essa é a chave que determina onde os arquivos estão localizados na api
para serem devidamente tratados.