using System;
using System.Net;
using System.Threading.Tasks;

namespace WebLib
{
    public static class Http
    {
        public static HttpResponse SendRequest(string url, HttpRequest options)
        {
            return SendRequestAsync(url, options).GetAwaiter().GetResult();
        }

        public static Task<HttpResponse> SendRequestAsync(string url, HttpRequest options)
        {
            options.Url = url;
            return SendRequestAsync(options);
        }

        public static HttpResponse SendRequest(HttpRequest options)
        {
            return SendRequestAsync(options).GetAwaiter().GetResult();
        }

        public static Task<HttpResponse> SendRequestAsync(HttpRequest options)
        {
            return SendRequestAsync(options.GetHttpWebRequest());
        }

        public static HttpResponse SendRequest(HttpWebRequest request)
        {
            return SendRequestAsync(request).GetAwaiter().GetResult();
        }

        public async static Task<HttpResponse> SendRequestAsync(HttpWebRequest request)
        {
            HttpResponse response;
            try
            {
                response = new HttpResponse((HttpWebResponse)await request.GetResponseAsync());
            }
            catch (WebException ex)
            {
                response = new HttpResponse((HttpWebResponse)ex.Response, true, ex);
            }

            return response;
        }
    }
}
