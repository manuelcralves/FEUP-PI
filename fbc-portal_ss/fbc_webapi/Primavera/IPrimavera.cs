using fbc_webapi.Classes;
using fbc_webapi.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using VndBE100;

namespace fbc_webapi.Primavera
{
    public class IPrimavera
    {
        public class Compras
        {
            
        }

        public class Internos 
        {

        }

        public class Utilitarios
        {

            public static string DaCaminhoDocumento(PrimaveraConnection pri, dynamic documento)
            {

                GerarPathFicheiroPdfTemporario(documento.Tipodoc, documento.Serie, documento.NumDoc, out string filePath, out string fileFolder, out string fileNameWithoutExtension, out string fileExtension, null);

                return filePath;
            }

            public static void GerarPathFicheiroPdfTemporario(string tipoDoc, string serie, int numDoc, out string tempFilePath, out string fileTempFolder, out string fileNameWithoutExtension, out string fileExtension, string tempFolder = null)
            {
                if (tempFolder == null)
                    tempFolder = Utils.GetTempFilesFolder();

                fileTempFolder = "";
                int tentativas = 0;
                while (string.IsNullOrEmpty(fileTempFolder) || (Directory.Exists(fileTempFolder) && tentativas < 5))
                {
                    fileTempFolder = System.IO.Path.Combine(tempFolder, Guid.NewGuid().ToString());
                    tentativas++;

                    if (tentativas == 5)
                        throw new FlorestasBemCuidadaWebApiException("Não foi possivel gerar pasta temporária depois de 5 tentativas.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                }

                Directory.CreateDirectory(fileTempFolder);

                try
                {
                    fileNameWithoutExtension = $"{tipoDoc}_{serie}_{numDoc}";
                    fileNameWithoutExtension = fileNameWithoutExtension.Replace("<", "").Replace(">", "").Replace(":", "").Replace("\"", "").Replace("/", "").Replace("\\", "").Replace("|", "").Replace("?", "").Replace("*", "");

                    fileExtension = ".pdf";

                    tempFilePath = System.IO.Path.Combine(fileTempFolder, fileNameWithoutExtension + fileExtension);

                    using (FileStream fileStream = File.Create(tempFilePath)) { }

                    Utils.TentarApagarFicheiro(tempFilePath);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(fileTempFolder);
                    }
                    catch (Exception) { }

                    throw;
                }
            }

        }

    }
}