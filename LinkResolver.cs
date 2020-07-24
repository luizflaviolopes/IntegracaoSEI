using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoSEI
{
    class LinkResolver
    {
        public static async Task<WebResponse> GetLinkResponse(string url)
        {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Proxy = new WebProxy("url do proxy", false);
            //webRequest.Proxy = WebRequest.GetSystemWebProxy();
            webRequest.ServicePoint.Expect100Continue = false;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;

            WebResponse asyncResult = webRequest.GetResponse();

                //StreamReader responseStream = new StreamReader(asyncResult.GetResponseStream());
                //string resultado = responseStream.ReadToEnd();

                return asyncResult;

        }
    }
}
