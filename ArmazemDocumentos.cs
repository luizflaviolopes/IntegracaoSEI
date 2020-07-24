using System;
using System.Collections.Generic;
using System.Net;

namespace IntegracaoSEI
{
    public class ArmazemDocumentos
    {
        public ArmazemDocumentos(string IdUnidade, string IdUnidadeNF)
        {
            new WebServices(IdUnidade, IdUnidadeNF);
        }

        public string getDocumentLink(string documentId)
        {
            var link = WebServices.getDocumentLink(documentId).GetAwaiter().GetResult();

            return link;

        }

        public string getDocumentProcess(string documentId)
        {
            try
            {
                var retorno = WebServices.getDocumentProcess(documentId).GetAwaiter().GetResult();
                return retorno;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public string CheckDocument(string documentId)
        {
            try
            {
                var retorno = WebServices.DocumentExists(documentId).GetAwaiter().GetResult();
                return retorno;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public WebResponse getDocument(string link)
        {
            var resultado = LinkResolver.GetLinkResponse(link).GetAwaiter().GetResult();

            return resultado;
        }

        public Dictionary<string,string> addDocument(string process, byte[] fileBytes, string tipo, string FileName = null)
        {
            TipoDocumento tipoEnum;

            if(!Enum.TryParse(tipo, out tipoEnum))
            {
                throw new Exception("Tipo Inválido");
            }

            var resultAdd = WebServices.InsertDocument(process, fileBytes, tipoEnum, FileName).GetAwaiter().GetResult();
            return resultAdd;
            
        }

        public bool delDocument(string docID)
        {
            try
            {
                var retorno = WebServices.delDocument(docID).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
