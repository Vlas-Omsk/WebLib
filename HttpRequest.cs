using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace WebLib
{
    public class HttpRequest : ICloneable
    {
        public const string Chrome79Win10UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.118 Safari/537.36";
        public const string Firefox95Win10UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";

        public const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";
        public const string JsonContentType = "application/json";

        public string Url { get; set; }
        public List<string> Path { get; set; } = new List<string>();
        public Dictionary<string, string> Query { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public string Method { get; set; }
        public Stream Data { get; set; }
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        public void SetHeader(string name, object value)
        {
            Headers[name] = value;
        }

        public void SetQueryParam(string key, string value)
        {
            Query[key] = value.ToString();
        }

        public void SetQueryParam(string key, object value)
        {
            SetQueryParam(key, value.ToString());
        }

        public void SetData(byte[] data)
        {
            Data = new MemoryStream(data);
        }

        public void SetData(string data, Encoding encoding)
        {
            if (string.IsNullOrEmpty(data))
                return;
            if (encoding is null)
                encoding = Encoding.UTF8;
            SetData(encoding.GetBytes(data));
        }

        public void SetData(Dictionary<string, string> collection, Encoding encoding)
        {
            if (collection.Count > 0)
                SetData(CreateQueryString(collection), encoding);
        }

        public void SetPath(string path)
        {
            Path = new List<string>(path.Trim('/').Split('/'));
        }

        private static string CreateQueryString(Dictionary<string, string> collection)
        {
            string result = string.Empty;

            if (collection.Count == 0)
                return result;

            var i = 0;
            foreach (var param in collection)
            {
                result += Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(param.Value);
                i++;
                if (i != collection.Count)
                    result += "&";
            }

            return result;
        }

        public HttpWebRequest GetHttpWebRequest()
        {
            if (string.IsNullOrEmpty(Url))
                throw new UriFormatException("Empty url");
            string url = Url + 
                /*(Url[Url.Length - 1] == '/' ? string.Empty : "/") +*/ 
                string.Join("/", Path.Select(item => Uri.EscapeDataString(item)));
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                throw new UriFormatException();
            if (Query.Count > 0)
                url += "?" + CreateQueryString(Query);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (string.IsNullOrEmpty(Method))
                throw new Exception("Empty method name");
            request.Method = Method;

            if (Headers.Count > 0)
                foreach (var header in Headers)
                    request.SetOption(header.Key, header.Value);

            request.CookieContainer = CookieContainer;

            if (Data != null && Data.Length > 0 &&
                !Method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                !Method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.ContentType))
                    throw new Exception("Empty Content-Type");

                request.ContentLength = Data.Length;
                using (var stream = request.GetRequestStream())
                    Data.CopyTo(stream);
            }

            return request;
        }

        public HttpResponse GetResponse()
        {
            return Http.SendRequest(this);
        }

        public async Task<HttpResponse> GetResponseAsync()
        {
            return await Http.SendRequestAsync(this);
        }

        public HttpRequest Clone()
        {
            return (HttpRequest)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public static HttpRequest GET
        {
            get
            {
                var options = new HttpRequest()
                {
                    Method = "GET"
                };
                
                return options;
            }
        }
        public static HttpRequest POST
        {
            get
            {
                var options = new HttpRequest()
                {
                    Method = "POST"
                };

                return options;
            }
        }
    }
}
