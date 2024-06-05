using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.ErrorHandling;
using System.ComponentModel.DataAnnotations;
using fbc_webapi.Models;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using static System.Net.Mime.MediaTypeNames;
using System.Web.Services.Description;
using Dapper;
using fbc_webapi.Classes.DataAccessLayer;
using Serilog.Core;
using System.Text;
using static BasBE100.BasBETiposGcp;
using System.Xml.Linq;

namespace fbc_webapi.Classes
{
    public static class Utils
    {
        public static string TemplateEmail(string Titulo, string Mensagem)
        {
            StringBuilder template = new StringBuilder();

            template.AppendLine("<!doctype html><html>  <head>    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />    <style>            img {        border: none;        -ms-interpolation-mode: bicubic;        max-width: 100%;       }      body {        background-color: #f6f6f6;        font-family: sans-serif;        -webkit-font-smoothing: antialiased;        font-size: 14px;        line-height: 1.4;        margin: 0;        padding: 0;        -ms-text-size-adjust: 100%;        -webkit-text-size-adjust: 100%;       }      table {        border-collapse: separate;        mso-table-lspace: 0pt;        mso-table-rspace: 0pt;        width: 100%; }        table td {          font-family: sans-serif;          font-size: 14px;          vertical-align: top;       }      .body {        background-color: #f6f6f6;        width: 100%;       }      .container {        display: block;        margin: 0 auto !important;        /* makes it centered */        max-width: 580px;        padding: 10px;        width: 580px;       }      .content {        box-sizing: border-box;        display: block;        margin: 0 auto;        max-width: 580px;        padding: 10px;       }      .main {        background: #ffffff;        border-radius: 3px;        width: 100%;       }      .wrapper {        box-sizing: border-box;        padding: 20px;       }      .header {        display: block;        margin: 10px;        margin-left: auto;        margin-right: auto;        width: 30%;      }          .content-block {        padding-bottom: 10px;        padding-top: 10px;      }      .footer {        clear: both;        margin-top: 10px;        text-align: center;        width: 100%;       }        .footer td,        .footer p,        .footer span,        .footer a {          color: #999999;          font-size: 12px;          text-align: center;       }      h1,      h2,      h3,      h4 {        color: #000000;        font-family: sans-serif;        font-weight: 400;        line-height: 1.4;        margin: 0;        margin-bottom: 30px;       }      h1 {        font-size: 35px;        font-weight: 300;        text-align: center;        text-transform: capitalize;       }      p,      ul,      ol {        font-family: sans-serif;        font-size: 14px;        font-weight: normal;        margin: 0;        margin-bottom: 15px;       }        p li,        ul li,        ol li {          list-style-position: inside;          margin-left: 5px;       }      a {        color: #3498db;        text-decoration: underline;       }      .btn {        box-sizing: border-box;        width: 100%; }        .btn > tbody > tr > td {          padding-bottom: 15px; }        .btn table {          width: auto;       }        .btn table td {          background-color: #ffffff;          border-radius: 5px;          text-align: center;       }        .btn a {          background-color: #ffffff;          border: solid 1px #3498db;          border-radius: 5px;          box-sizing: border-box;          color: #3498db;          cursor: pointer;          display: inline-block;          font-size: 14px;          font-weight: bold;          margin: 0;          padding: 12px 25px;          text-decoration: none;          text-transform: capitalize;       }      .btn-primary table td {        background-color: #3498db;       }      .btn-primary a {        background-color: #3498db;        border-color: #3498db;        color: #ffffff;       }      .last {        margin-bottom: 0;       }      .first {        margin-top: 0;       }      .align-center {        text-align: center;       }      .align-right {        text-align: right;       }      .align-left {        text-align: left;       }      .clear {        clear: both;       }      .mt0 {        margin-top: 0;       }      .mb0 {        margin-bottom: 0;       }  .powered-by a {        text-decoration: none;       }      hr {        border: 0;        border-bottom: 1px solid #f6f6f6;        margin: 20px 0;       }      @media only screen and (max-width: 620px) {        table.body h1 {          font-size: 28px !important;          margin-bottom: 10px !important;         }        table.body p,        table.body ul,        table.body ol,        table.body td,        table.body span,        table.body a {          font-size: 16px !important;         }        table.body .wrapper,        table.body .article {          padding: 10px !important;         }        table.body .content {          padding: 0 !important;         }        table.body .container {          padding: 0 !important;          width: 100% !important;         }        table.body .main {          border-left-width: 0 !important;          border-radius: 0 !important;          border-right-width: 0 !important;         }        table.body .btn table {          width: 100% !important;         }        table.body .btn a {          width: 100% !important;         }        table.body .img-responsive {          height: auto !important;          max-width: 100% !important;          width: auto !important;         }      }      @media all {        .ExternalClass {          width: 100%;         }        .ExternalClass,        .ExternalClass p,        .ExternalClass span,        .ExternalClass font,        .ExternalClass td,        .ExternalClass div {          line-height: 100%;         }        .apple-link a {          color: inherit !important;          font-family: inherit !important;          font-size: inherit !important;          font-weight: inherit !important;          line-height: inherit !important;          text-decoration: none !important;         }        #MessageViewBody a {          color: inherit;          text-decoration: none;          font-size: inherit;          font-family: inherit;          font-weight: inherit;          line-height: inherit;        }        .btn-primary table td:hover {          background-color: #34495e !important;         }        .btn-primary a:hover {          background-color: #34495e !important;          border-color: #34495e !important;         }       }    </style>  </head>  <body> <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"body\">      <tr>        <td>&nbsp;</td>        <td class=\"container\">          <div class=\"content\">            <table role=\"presentation\" class=\"main\">              <tr>                                <td class=\"wrapper\">                  <table align=\"center\" width=\"75%\">                    <tr>                        <th>                            <div style=\"margin: 0 auto; text-align: center; margin-bottom: 20px;\">                                <img align=\"center\" src=\"https://media.licdn.com/dms/image/C4D12AQHGwMfmgxzpmQ/article-cover_image-shrink_180_320/0/1623428829856?e=2147483647&v=beta&t=IUCBK-R4NYojt95P6qyhyjspUJGvUD1JKP2WJU_f7ag\" width=\"100\"/>                            </div>                        </th>                    </tr>                </table>                  <table align=\"center\" style=\"margin: 10px auto;\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">                    <tr>                      <td>");

            template.AppendFormat("<h3>{0}<h3>", Titulo);

            template.AppendFormat("<p>{0}</p>", Mensagem);

            template.AppendLine("</td></tr></table></td></tr></table><div class=\"footer\"><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"content-block\"><a href=\"http://florestabemcuidada.ddns.net:3000/\">FlorestaBemCuidada<span style=\"color: #00751F\">.portal</span></a>                  </td>                </tr>                <tr>                  <td class=\"content-block powered-by\">                    <span class=\"grey\">Notificação enviada pelo portal.</span>                  </td>                </tr>              </table>            </div>          </div>        </td>        <td>&nbsp;</td>      </tr>    </table>  </body></html>");
            //template.AppendLine("<!doctype html><html><head><meta charset=\"utf-8\"><style> body {font-family: Helvetica, Arial, sans-serif;font-size: 14px; line-height: 21px; color: #23232C;padding: 0;margin: 0;background:#DFDFDF;}a{color: inherit; text-decoration: none;letter-spacing: 0.5px;margin: 0 4px;} .wrapper {max-width: 560px;margin: 0 auto;background: #fff;} .header {background: #00751F; text-align: center; display: block; padding: 22px 20px 16px; margin: 0;} .footer {background: #000000; color: #A0A0AC; text-align: center; padding: 24px;} .footer span {font-size: 12px;} .footer a { color: #fff; letter-spacing: 1px; margin: 8px 10px 8px; display: inline-block;} .label {width: 60px;display: inline-block} .container {max-width: 460px; padding: 32px 20px 60px; margin: 0 auto;} h1 {font-size: 24px; line-height: 36px;} </style></head><body><div class=\"wrapper\"><a href=\"https://www.florestabemcuidada.com/\" class=\"header\"><img style=\"height:100px\" src=\"https://media.licdn.com/dms/image/C4D12AQHGwMfmgxzpmQ/article-cover_image-shrink_180_320/0/1623428829856?e=2147483647&v=beta&t=IUCBK-R4NYojt95P6qyhyjspUJGvUD1JKP2WJU_f7ag\"/></a><div class=\"container\">");

            return template.ToString();
        }


        public static void EnviaNotificacaoLeaders(string Utilizador, dynamic Documento)
        {
            try
            {
                var utilizador =  AcessosDAL.GetUtilizador(Utilizador).Result;

                if (utilizador == null)
                    throw new Exception("O utilizador '{Utilizador}' não foi encontrado.");
      
               var equipas =  BaseDAL.GetEquipasMembro(Utilizador).Result;

                if (equipas == null)
                    throw new Exception("Não foram encontradas equipas para o utilizador '{Utilizador}'.");
               
              foreach(var equipa in equipas)
              {
                var Equipa = BaseDAL.GetEquipa(equipa.Codigo).Result;

                foreach (var leader in Equipa.Leaders)
                {     
                   EnviaNotificacaoLeader(utilizador, leader, Documento);
                }
              }

            }
            catch (Exception ex)
            {
                Log.Debug("EnviaNotificacaoLeaders | {Message}", ex.Message);
            }
        }

        public static void EnviaNotificacaoLeader(Utilizador utilizador, Leader leader, dynamic documento)
        {
            var leaderPerfil =  AcessosDAL.GetUtilizador(leader.Utilizador).Result;

            if (leaderPerfil == null){

                Log.Debug("EnviaNotificacaoLeader | O leader '{Utilizador}' não foi encontrado.'.", leader.Utilizador);
                return;
            }

            string Titulo = $"O documento '{documento.Documento}' necessita da sua aprovação!";
            string Mensagem = $"Foi submetido pelo utilizador {utilizador.Nome}, um documento  do tipo {documento.TipoDoc} na data {DateTime.Now}.";

            var email = new Email()
            {
                Utilizador = utilizador.Codigo,    
                To = leaderPerfil.Email,       
                Assunto = $"Documento para aprovação | {documento.Documento}",
                Mensagem = TemplateEmail(Titulo,Mensagem),
                Formato = true
            };

            InsertMensagensEmail(email);        
        }

        public static void EnviaNotificacaoAprovado(string Aprovador, dynamic DocumentoModel, dynamic Documento, string filePath)
        {
            try
            {        
                if (filePath == null) 
                    throw new Exception("O caminho do documento não foi encontrado.'.");
 
                var utilizador = AcessosDAL.GetUtilizador(DocumentoModel.Utilizador).Result;

                if (utilizador == null)
                    throw new Exception($"O Utilizador '{DocumentoModel.Utilizador}' não foi encontrado.'.");

                var aprovador = AcessosDAL.GetUtilizador(Aprovador).Result;

                if (aprovador == null)
                    throw new Exception($"O Aprovador '{Aprovador}' não foi encontrado.'.");

                var equipas = BaseDAL.GetEquipasLeader(Aprovador).Result;

                if (equipas == null)
                    throw new Exception($"Não foram encontradas equipas para o aprovador '{Aprovador}'.");

                EnviaNotificacaoUtilizadorAprovado(aprovador, utilizador.Email, Documento, filePath);

                foreach (var equipa in equipas)
                    EnviaNotificacaoEquipaAprovado(aprovador, equipa, Documento, filePath);

            }
            catch (Exception ex)
            {
                Log.Debug("EnviaNotificacaoAprovado | {Message}", ex.Message);
            }
        }

        public static void EnviaNotificacaoRejeitado(string Aprovador, dynamic Documento)
        {
            try
            {
                var aprovador = AcessosDAL.GetUtilizador(Aprovador).Result;

                var utilizador = AcessosDAL.GetUtilizador(Documento.Utilizador).Result;

                if (aprovador == null && utilizador == null)
                    throw new Exception($"O Utilizador '{Aprovador}' não foi encontrado.'.");

                var equipas = BaseDAL.GetEquipasLeader(Aprovador).Result;

                if (equipas == null)
                    throw new Exception($"Não foram encontradas equipas para o utilizador '{Aprovador}'.");


                foreach (var equipa in equipas)
                    EnviaNotificacaoEquipaRejeitado(aprovador, utilizador, equipa, Documento);
            }
            catch (Exception ex)
            {
                Log.Debug("EnviaNotificacaoRejeitado | {Message}", ex.Message);
            }
        }



        internal static void EnviaNotificacaoEquipaRejeitado(Utilizador aprovador, Utilizador utilizador, Equipa equipa, dynamic documento)
        {   
            //Email Aprovador
            var Assunto = $"Documento rejeitado | {documento.Documento}";

            var Titulo = "Um documento foi rejeitado !";

            var Mensagem = new StringBuilder();
            Mensagem.AppendLine($"Foi rejeitado pelo utilizador {aprovador.Nome}, o documento {documento.Documento} gerado na data {documento.DataDoc.Value.Date}.");
            Mensagem.AppendLine($"<br><br><b>Data Rejeição</b> : {DateTime.Now}");
            Mensagem.AppendLine($"<br><br><b>Motivo Rejeição</b> : {documento.MotivoRejeicao}");

            EnviaNotificacao(aprovador.Codigo, equipa.Email, Assunto, Titulo, Mensagem.ToString());


            // Email Utilizador
            Titulo = "O Seu documento foi rejeitado !";

            Mensagem = new StringBuilder();
            Mensagem.AppendLine($"Foi rejeitado pelo utilizador {aprovador.Nome}, o seu documento {documento.Documento} gerado na data {documento.DataDoc.Value.Date}.");
            Mensagem.AppendLine($"<br><br><b>Data Rejeição</b> : {DateTime.Now}");
            Mensagem.AppendLine($"<br><br><b>Motivo Rejeição</b> : {documento.MotivoRejeicao}");

            EnviaNotificacao(aprovador.Codigo, utilizador.Email, Assunto, Titulo, Mensagem.ToString());
        }

        internal static void EnviaNotificacaoUtilizadorAprovado(Utilizador aprovador, string email, dynamic documento, string filePath = null)
        {
            var Assunto = $"Documento aprovado | {documento.Documento}";
            var Titulo = $"O seu documento foi aprovado !";
            var Mensagem = $"Foi aprovado pelo utilizador {aprovador.Nome}, o seu documento {documento.Documento} na data {DateTime.Now}.";

             EnviaNotificacao(aprovador.Codigo, email, Assunto, Titulo, Mensagem, filePath);
        }

        internal static void EnviaNotificacaoEquipaAprovado(Utilizador aprovador, Equipa equipa, dynamic documento, string filePath = null)
        {
            var Assunto = $"Documento aprovado | {documento.Documento}";
            var Titulo = $"Um novo documento foi aprovado !";
            var Mensagem = $"Foi aprovado pelo utilizador {aprovador.Nome}, o documento {documento.Documento} na data {DateTime.Now}.";

             EnviaNotificacao(aprovador.Codigo, equipa.Email, Assunto, Titulo, Mensagem, filePath);
        }

        internal static void EnviaNotificacao(string utilizador, string destinatario, string assunto, string titulo, string mensagem, string filePath = null)
        {
            var email = new Email()
            {
                Utilizador = utilizador,
                To = destinatario,
                Assunto = assunto,
                Mensagem = TemplateEmail(titulo, mensagem),
                Formato = true,
                Anexos = filePath
            };

             InsertMensagensEmail(email);
        }

        public static void InsertMensagensEmail(Email email)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                         conn.Query(@"INSERT INTO [MensagensEmail]
                                               ([From]
                                               ,[To]
                                               ,[CC]
                                               ,[BCC]
                                               ,[Assunto]
                                               ,[Mensagem]
                                               ,[Anexos]
                                               ,[Formato]
                                               ,[Utilizador])

                                        VALUES (@From
                                               ,@To
                                               ,@CC
                                               ,@BCC
                                               ,@Assunto
                                               ,@Mensagem
                                               ,@Anexos
                                               ,@Formato
                                               ,@Utilizador)", email, tran);

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }

            }
        }

        public static async Task<List<string>> GetFicheirosEnviados(string fieldName)
        {
            List<string> novosAnexos = new List<string>();

            try
            {
                // guardar ficheiros em pastas temporarias
                string tempFolder = Utils.GetTempFilesFolder();
                foreach (HttpPostedFile file in HttpContext.Current.Request.Files.GetMultiple(fieldName))
                {
                    string fileTempFolder = "";
                    int tentativas = 0;
                    while (string.IsNullOrEmpty(fileTempFolder) || (Directory.Exists(fileTempFolder) && tentativas < 5))
                    {
                        fileTempFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString());
                        tentativas++;

                        if (tentativas == 5)
                            throw new FlorestasBemCuidadaWebApiException("Não foi possivel gerar pasta temporária depois de 5 tentativas.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                    }

                    Directory.CreateDirectory(fileTempFolder);

                    string tempFilePath = Path.Combine(fileTempFolder, file.FileName);

                    using (FileStream fileStream = File.Create(tempFilePath))
                        await file.InputStream.CopyToAsync(fileStream);

                    novosAnexos.Add(tempFilePath);
                }
            }
            catch (Exception)
            {
                if (novosAnexos.Count > 0)
                    TentarApagarFicheirosTemporarios(novosAnexos);

                throw;
            }

            return novosAnexos;
        }

        public static void TentarApagarFicheirosTemporarios(List<string> ficheiros)
        {
            if (ficheiros != null)
            {
                foreach (string ficheiro in ficheiros)
                    TentarApagarFicheiroTemporario(ficheiro);
            }
        }

        public static void TentarApagarFicheiroTemporario(string ficheiro)
        {
            TentarApagarFicheiro(ficheiro);

            try
            {
                DirectoryInfo parentDirectory = Directory.GetParent(ficheiro);

                if (Guid.TryParse(parentDirectory.Name, out _))
                {
                    string tempFolder = GetTempFilesFolder();

                    // só apagar se pasta estiver em formato GUID e estiver dentro da pasta temporária
                    // evita apagar outras pastas por engano
                    if (parentDirectory?.Parent?.FullName.Equals(tempFolder, StringComparison.OrdinalIgnoreCase) == true)
                        Directory.Delete(parentDirectory.FullName);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Erro a apagar pasta de ficheiro temporário {PathFicheiro}.", ficheiro);
            }
        }

        public static void TentarApagarFicheiros(List<string> ficheiros)
        {
            if (ficheiros != null)
            {
                foreach (string ficheiro in ficheiros)
                    TentarApagarFicheiro(ficheiro);
            }
        }

        public static void TentarApagarFicheiro(string ficheiro)
        {
            try
            {
                if (!string.IsNullOrEmpty(ficheiro))
                    File.Delete(ficheiro);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Erro a apagar ficheiro '{PathFicheiro}'. Pode não ser um problema.", ficheiro);
            }
        }

        public static string GetTempFilesFolder()
        {
            if (HttpContext.Current == null)
                throw new FlorestasBemCuidadaWebApiException("HttpContext.Current está a null. Normalmente acontece por esta função ter sido executada numa thread diferente da que recebeu o pedido no controller, por exemplo por usar métodos async com ConfigureAwait(false).", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            string tempFolder = HttpContext.Current.Server.MapPath("~/App_Data/Temp");

            if (string.IsNullOrEmpty(tempFolder))
                throw new FlorestasBemCuidadaWebApiException("tempFolder sem valor", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            return tempFolder;
        }
    }
}