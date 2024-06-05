
Data: 22/02/2022

----------------------------------------------------------------------------------------------------
CONFIGURAÇÕES
----------------------------------------------------------------------------------------------------

Além do "Web.config" (que é renomeado para "fbc_webapi.dll.config" ao compilar/publicar),
também existe um "ApplicationSettings.config" e um "appsettings.json".

O "Web.config"/"fbc_webapi.dll.config" tem somente configurações de IIS, sistema e runtime.

O "ApplicationSettings.config" tem as configurações da aplicação, e é normalmente o que precisa de
alterações, especialmente ao usar em novas máquinas.

Como o "ApplicationSettings.config" tem configurações que variam de máquina para máquina, não deve
ser enviado para o repositório GIT, já que os desenvolvedores iriam sobrepor as configurações
dos outros.

O ficheiro "ApplicationSettings.config.example" deve ser mantido com configurações exemplo para
novos desenvolvedores poderem criar o seu próprio ApplicationSettings.config mais facilmente.
E também para em novas instalações em servidor de cliente termos uma base para configurar.

Ao correr no Visual Studio, apesar do "Web.Config" ser copiado para a pasta bin, o
"ApplicationSettings.config" não é, nem precisa. O "ApplicationSettings.config" é lido da pasta
base.

O ficheiro "appsettings.json" só tem configurações de logging, usados pelo Serilog.

Os ficheiros de configurações não devem ser publicados para evitar sobrepor ficheiros no servidor
do cliente.

Em vez de publicar os ficheiros de configuração, deve-se publicar os ficheiros ".example" para
em novas instalações termos um exemplo/ficheiro base para instalar e configurar mais facilmente.

----------------------------------------------------------------------------------------------------
AUTENTICAÇÃO
----------------------------------------------------------------------------------------------------

O sistema de autenticação usado é OAuth 2.0 com Password Grant.

O ClientId deve ser configurado no "ApplicationSettings.config".
O ClientSecret não é usado.

Username e password são os usados no ERP Primavera, utilizando motores para valida-los.

A maior parte das classes responsáveis pela autenticação estão na pasta "Autenticacao".

No caso de download de ficheiros, já que o browser não envia o token no cabeçalho e meter no URL
não era seguro, usa-se um sisteme de tokens de download.
Primeiro faz-se um pedido de gerar token de download, com o token de autenticação, e depois o
browser pode enviar um pedido de download de ficheiro com o token de download no URL.
Já que esse token só serve para fazer download desse tipo de ficheiro e só pode ser usado uma vez,
é mais seguro apesar de ser exposto no URL.

----------------------------------------------------------------------------------------------------
ACESSO A BASE DE DADOS
----------------------------------------------------------------------------------------------------

A aplicação não tem base de dados própria. É usado bases de dados do Primavera.

Os dados usados para ligação à base de dados são os fornecidos pelos motores Primavera.
Os ficheiros de configuração desta aplicação só têm alguns dados gerais para aceder aos motores.

A classe Config gere as connections strings obtidas dos motores.

----------------------------------------------------------------------------------------------------
BIBLIOTECAS PRIMAVERA
----------------------------------------------------------------------------------------------------

É importante que todas as bibliotecas Primavera sejam adicionadas com "Copy Local" a false e
"Embed Interop Types" a false para evitar conflitos com os dlls da instalação Primavera.

As bibliotecas Primavera a usar devem ser copiadas para "fbc_webapi\Libraries" e as 
referências adicionadas de lá. Desta forma evita-se conflitos quando desenvolvedores diferentes
têm o Primavera instalado em diretorias diferentes. Tambem é possivel alguem compilar sem ter o
Primavera instalado.

Durante a execução a aplicação usa um "AssemblyResolver" no Global.asax para forçar a usar as dlls
da pasta da instalação Primavera.
Para encontrar a pasta do Primavera é usado o algoritmo da função
"PrimaveraUtils.FindPrimaveraV10Path()".
Essa função procura primeiro em variaveis de ambiente do utilizador e da máquina, depois no registo
do Windows e finalmente tenta a diretoria de instalação Primavera "genérica" dentro de Program Files.

----------------------------------------------------------------------------------------------------
ACESSO A MOTORES
----------------------------------------------------------------------------------------------------

A maior parte da interação com motores passa pela classe "PrimaveraConnection", que tem as
propriedades BSO e PSO para interagir com o ErpBS e StdPlatBS.

A classe "PrimaveraConnection" precisa que lhe seja passado um "ConfiguracoesLigacaoPrimavera" ou um
"ConfiguracoesLigacaoEmpresaPrimavera" para conseguir abrir os motores. O primeiro só é usado para
abrir os motores da plataforma sem autenticação, só tendo acesso a algumas funcionalidades. O 
segundo usa autenticação de utilizador, normalmente para uma empresa especifica.

A classe "ConfiguracoesLigacaoPrimaveraFactory" pode ser usada para obter um
"ConfiguracoesLigacaoPrimavera" ou "ConfiguracoesLigacaoEmpresaPrimavera" mais facilmente.

----------------------------------------------------------------------------------------------------
LOGGING
----------------------------------------------------------------------------------------------------

Para logs é usado o Serilog, com uma classe estática Serilog.Log para acesso fácil a logs em
qualquer parte da aplicação.

As configurações dele ficam em "appsettings.json" que são carregadas no arranque da aplicação,
no Global.asax.
O sistema de configurações do Serilog é muito flexivel e a maior parte pode ser configurado sem
precisar de mexer em código.

A maior parte da documentação de configurações pode ser consultada aqui:
https://github.com/serilog/serilog-settings-configuration

No caso de alguns módulos do Serilog, pode ser preciso consultar a documentação de configurações
dos respetivos módulos nas páginas dos próprios módulos.
Por exemplo, a documentação do módulo de logs para ficheiro fica aqui:
https://github.com/serilog/serilog-sinks-file

----------------------------------------------------------------------------------------------------
TRATAMENTO DE ERROS
----------------------------------------------------------------------------------------------------

A maior parte das classes relativa a tratamento de erros ficam na pasta "ErrorHandling".

Também existe algum em Global.asax caso alguma exceção chege ao evento "Application_Error" ou se
ocorrerem erros no carregamento inicial da aplicação.

Normalmente, quando detetarmos alguma situação inválida, é levantado um
"FlorestasBemCuidadaWebApiException", que se não for apanhado será tratado na classe
"GlobalExceptionHandler" onde converte num "ErrorResponse" e devolve-o na resposta web.

Em alguns casos a exceção é apanhada pela classe "OwinExceptionHandlerMiddleware" em vez de
"GlobalExceptionHandler", quando a exceção é levantada em Middlewares da Owin, como na
autenticação de pedidos.

O "FlorestasBemCuidadaWebApiException" permite identificar se a nossa mensagem é "user friendly" e nesse
caso a mensagem é devolvida no ErrorResponse. Caso contrário usa uma mensagem genérica de ter
ocorido um erro no servidor.
Também permite identificar um "HttpStatusCode" se quisermos algo diferente do erro "500" de
"Internal Server Error".

----------------------------------------------------------------------------------------------------
SCRIPTS
----------------------------------------------------------------------------------------------------

Scripts SQL são adicionados à pasta "Scripts" e publicados junto com a API.
Será preciso corre-los manualmente em novas instalações/em novas bases de dados Primavera.
Não existe sistema automático para correr Scripts.

----------------------------------------------------------------------------------------------------
SWAGGER
----------------------------------------------------------------------------------------------------

Alguns endpoints precisam de configuração manual para aparecerem bem no Swagger.
Essas configurações de endpoints ficam em "App_Start\SwaggerOperationFilters".

Por exemplo, o post (criação) e put (alteração) de propostas abertas precisa de operation filters
para mostrar os parametros multipart/form-data.
