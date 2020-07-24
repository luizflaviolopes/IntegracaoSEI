using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;

namespace IntegracaoSEI
{
    class WebServices
    {
        static string idServico = "IdDoServiço - (pegar com gestores do SEI)";
        static string IdUnidade;
        static string IdUnidadeNF;
        static string IdSistema = "IdDoSistema - (pegar com gestores do SEI)";

        static string _action = "\"SeiAction\"";
        static string _url = "http://sei.mg.gov.br/sei/ws/SeiWS.php";

        public WebServices(string Id, string IdNF)
        {
            IdUnidade = Id;
            IdUnidadeNF = IdNF;
        }


        public static async Task<string> getDocumentLink(string documentId)
        {
            var responseXml = documentSearch(documentId, IdUnidade);
            var link = responseXml.GetElementsByTagName("LinkAcesso")[0];
            return link.InnerText;
        }

        public static async Task<string> getDocumentProcess(string documentId)
        {
            var responseXml = documentSearch(documentId, IdUnidade);
            var link = responseXml.GetElementsByTagName("ProcedimentoFormatado")[0];
            return link.InnerText;
        }

        public static async Task<string> DocumentExists(string documentId)
        {
            var responseXml = documentSearch(documentId, IdUnidadeNF);
            var link = responseXml.GetElementsByTagName("ProcedimentoFormatado")[0];
            return link.InnerText;
        }

        public static async Task<Dictionary<string, string>> InsertDocument(string process, byte[] fileBytes, TipoDocumento tipo, string FileName = null)
        {
            var result = await CallInsertDocument(process, fileBytes, tipo, FileName);
            var idDoc = result.GetElementsByTagName("DocumentoFormatado")[0];
            var link = result.GetElementsByTagName("LinkAcesso")[0];

            return new Dictionary<string, string> { { "Id", idDoc.InnerText }, { "Link", link.InnerText } };
        }

        public static async Task<string> delDocument(string documentId)
        {
            var responseXml = await callCancelDoc(documentId, IdUnidadeNF);
            var link = responseXml.GetElementsByTagName("parametros")[0];
            return link.InnerText;
        }


        private static async Task<XmlDocument> CallInsertDocument(string process, byte[] fileBytes, TipoDocumento tipo, string FileName = null)
        {
            Documento documento = new Documento(process, tipo, fileBytes, FileName);

            XmlDocument soapEnvelopeXml = CreateSoapInsertDocument(documento);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            WebResponse asyncResult = webRequest.GetResponse();

            string soapResult = ReadResponse(asyncResult);
            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(soapResult);
            return responseXml;

        }

        private static XmlDocument documentSearch(string documentId, string idUnidade)
        {

            XmlDocument soapEnvelopeXml = CreateSoapGetDocument(documentId, idUnidade);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            WebResponse asyncResult = webRequest.GetResponse();

            // get the response from the completed web request.
            string soapResult = ReadResponse(asyncResult);

            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(soapResult);
            return responseXml;
        }


        private static async Task<XmlDocument> callCancelDoc(string documentId, string idUnidade)
        {

            XmlDocument soapEnvelopeXml = CreateSoapDelDocument(documentId, idUnidade);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            WebResponse asyncResult = webRequest.GetResponse();

            // get the response from the completed web request.
            string soapResult = ReadResponse(asyncResult);
            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(soapResult);
            return responseXml;
        }



        private static XmlDocument CreateSoapInsertDocument(Documento arquivo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:sei=""Sei"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"">");
            sb.Append(@"<soapenv:Header/>");
            sb.Append(@"<soapenv:Body>");
            sb.Append(@"<sei:incluirDocumento soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">");
            sb.Append(@"<SiglaSistema xsi:type=""xsd: string"">").Append(IdSistema).Append("</SiglaSistema>");
            sb.Append(@"<IdentificacaoServico xsi:type=""xsd: string"">").Append(idServico).Append("</IdentificacaoServico>");
            sb.Append(@"<IdUnidade xsi:type=""xsd: string"">").Append(IdUnidade).Append("</IdUnidade>");
            sb.Append(@"<Documento xsi:type=""sei: Documento"">");
            sb.Append(@"<Tipo xsi:type=""xsd: string"">").Append(arquivo.Tipo).Append("</Tipo>");
            sb.Append(@"<IdProcedimento xsi:type=""xsd: string"">").Append("").Append("</IdProcedimento>");
            sb.Append(@"<ProtocoloProcedimento xsi:type=""xsd: string"">").Append(arquivo.ProtocoloProcedimento).Append("</ProtocoloProcedimento>");
            sb.Append(@"<IdSerie xsi:type=""xsd: string"">").Append(arquivo.IdSerie).Append("</IdSerie>");
            sb.Append(@"<Numero xsi:type=""xsd: string"">").Append(arquivo.Numero).Append("</Numero>");
            sb.Append(@"<Data xsi:type=""xsd: string"">").Append(arquivo.Data).Append("</Data>");
            sb.Append(@"<Descricao xsi:type=""xsd: string"">").Append(arquivo.Descricao).Append("</Descricao>");
            //sb.Append(@"<IdTipoConferencia xsi:type=""xsd: string"">").Append(arquivo.IdTipoConferencia).Append("</IdTipoConferencia>");
            sb.Append(@"<Remetente xsi:type=""sei: Remetente"">");
            sb.Append(@"<Sigla xsi:type=""xsd: string"">").Append(arquivo.RemetenteSigla).Append("</Sigla>");
            sb.Append(@"<Nome xsi:type=""xsd: string"">").Append(arquivo.RemetenteNome).Append("</Nome>");
            sb.Append(@"</Remetente>");
            sb.Append(@"<Interessados xsi:type=""sei: ArrayOfInteressado"" soapenc:arrayType=""sei: Interessado[]""/>");
            sb.Append(@"<Destinatarios xsi:type=""sei: ArrayOfDestinatario"" soapenc:arrayType=""sei: Destinatario[1]"">");
            sb.Append(@"<Destinatario>");
            sb.Append(@"<Sigla xsi:type=""xsd: string"">").Append(arquivo.DestinatariosSigla).Append("</Sigla>");
            sb.Append(@"<Nome xsi:type=""xsd: string"">").Append(arquivo.DestinatariosNome).Append("</Nome>");
            sb.Append(@"</Destinatario>");
            sb.Append(@"</Destinatarios>");
            sb.Append(@"<Observacao xsi:type=""xsd: string"">").Append(arquivo.Observacao).Append("</Observacao>");
            sb.Append(@"<NomeArquivo xsi:type=""xsd: string"">").Append(arquivo.Nome).Append("</NomeArquivo>");
            sb.Append(@"<NivelAcesso xsi:type=""xsd: string"">").Append(arquivo.NivelAcesso).Append("</NivelAcesso>");
            sb.Append(@"<IdHipoteseLegal xsi:type=""xsd: string"">").Append(arquivo.HipoteseLegal).Append("</IdHipoteseLegal>");
            sb.Append(@"<Conteudo xsi:type=""xsd: string"">").Append(arquivo.Base64).Append("</Conteudo>");
            //sb.Append(@"<ConteudoMTOM xsi:type=""xsd: base64Binary""></ConteudoMTOM>");
            //sb.Append(@"<IdArquivo xsi:type=""xsd: string"">").Append("").Append("</IdArquivo>");
            sb.Append(@"<Campos xsi:type=""sei: ArrayOfCampo"" soapenc:arrayType=""sei: Campo[]""/>");
            /*sb.Append(@"<Campos xsi:type=""sei: ArrayOfCampo"" soapenc:arrayType=""sei: Campo[2]"">");
                sb.Append(@"<Campo>");
                    sb.Append(@"<Nome xsi:type=""xsd: string"">").Append("@nome_destinatario@").Append("</Nome>");
                    sb.Append(@"<Valor xsi:type=""xsd: string"">").Append("Luiz Flávio Rodrigues Lopes").Append("</Valor>");
                sb.Append(@"</Campo>");
             sb.Append(@"</Campos>");*/
            sb.Append(@"<SinBloqueado xsi:type=""xsd: string"">").Append(arquivo.SinBloqueado).Append("</SinBloqueado>");
            sb.Append(@"</Documento>");
            sb.Append(@"</sei:incluirDocumento>");

            sb.Append(@"</soapenv:Body>");
            sb.Append(@"</soapenv:Envelope>");


            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(sb.ToString());
            return soapEnvelopeDocument;
        }

        private static XmlDocument CreateSoapGetDocument(string document, string unidade)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:sei=""Sei"">
           <soapenv:Header/>
           <soapenv:Body>
              <sei:consultarDocumento soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                 <SiglaSistema xsi:type=""xsd:string"">" + IdSistema + @"</SiglaSistema>
                 <IdentificacaoServico xsi:type=""xsd:string"">" + idServico + @"</IdentificacaoServico>
                 <IdUnidade xsi:type=""xsd:string"">" + unidade + @"</IdUnidade>
                 <ProtocoloDocumento xsi:type=""xsd:string"">" + document + @"</ProtocoloDocumento>
                 <SinRetornarAndamentoGeracao xsi:type=""xsd:string"">N</SinRetornarAndamentoGeracao>
                 <SinRetornarAssinaturas xsi:type=""xsd:string"">N</SinRetornarAssinaturas>
                 <SinRetornarPublicacao xsi:type=""xsd:string"">N</SinRetornarPublicacao>
                 <SinRetornarCampos xsi:type=""xsd:string"">N</SinRetornarCampos>
              </sei:consultarDocumento>
           </soapenv:Body>
        </soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static XmlDocument CreateSoapDelDocument(string document, string unidade)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:sei=""Sei"">
   <soapenv:Header/>
   <soapenv:Body>
      <sei:cancelarDocumento soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
         <SiglaSistema xsi:type=""xsd:string"">" + IdSistema + @"</SiglaSistema>
         <IdentificacaoServico xsi:type=""xsd:string"">" + idServico + @"</IdentificacaoServico>
         <IdUnidade xsi:type=""xsd:string"">" + unidade + @"</IdUnidade>
         <ProtocoloDocumento xsi:type=""xsd:string"">" + document + @"</ProtocoloDocumento>
         <Motivo xsi:type=""xsd:string"">Documento inválido</Motivo>
      </sei:cancelarDocumento>
   </soapenv:Body>
</soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            //webRequest.ContentLength = Encoding.ASCII.GetBytes(soapEnvelopeXml.OuterXml).Length;

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=UTF-8";
            webRequest.Accept = "*/*";
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            webRequest.Method = "POST";


            webRequest.KeepAlive = true;
            //webRequest.UserAgent = "Apache-HttpClient/4.1.1 (java 1.5)";
            //webRequest.Proxy = new WebProxy("url do proxy", false);
            //webRequest.Proxy = WebRequest.GetSystemWebProxy();
            webRequest.ServicePoint.Expect100Continue = false;



            return webRequest;
        }
        private static string ReadResponse(WebResponse wr)
        {
            string soapResult;
            using (WebResponse webResponse = wr)
            {
                if (wr.Headers["Content-Encoding"] == "gzip")
                {
                    using (StreamReader rd = new StreamReader(new System.IO.Compression.GZipStream(wr.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)))
                    {
                        soapResult = rd.ReadToEnd();
                    }

                }
                else
                {
                    using (StreamReader rd = new StreamReader(wr.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }               

            }
            return soapResult;
        }
    }
}
