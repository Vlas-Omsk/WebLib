using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace WebLib
{
    public class Web
    {
        public Response SendRequest(string url, RequestOptions options)
        {
            options.Url = url;
            return SendRequest(options);
        }

        public Response SendRequest(RequestOptions options)
        {
            return SendRequest(options.GetHttpWebRequest());
        }

        public Response SendRequest(HttpWebRequest request)
        {
            Response response = new Response();
            HttpWebResponse webresponse;
            try
            {
                webresponse = (HttpWebResponse)request.GetResponse();
            } catch (WebException ex)
            {
                webresponse = (HttpWebResponse)ex.Response;
                response.IsHaveErrors = true;
            }

            response.WebResponse = webresponse;
            using (Stream s = webresponse.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                    response.Content = sr.ReadToEnd();
            }

            return response;
        }
    }
}
