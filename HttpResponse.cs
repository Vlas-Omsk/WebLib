using System;
using System.IO;
using System.Net;

namespace WebLib
{
    public class HttpResponse : IDisposable
    {
        public HttpWebResponse HttpWebResponse { get; }
        public bool HaveErrors { get; }
        public WebHeaderCollection Headers => HttpWebResponse.Headers;
        public HttpStatusCode StatusCode => HttpWebResponse.StatusCode;

        public HttpResponse(HttpWebResponse httpWebResponse) : this(httpWebResponse, false)
        {
        }

        public HttpResponse(HttpWebResponse httpWebResponse, bool haveErrors)
        {
            HttpWebResponse = httpWebResponse;
            HaveErrors = haveErrors;
        }

        public string GetText()
        {
            using (Stream stream = GetStream())
            {
                using (StreamReader streamReader = new StreamReader(stream))
                    return streamReader.ReadToEnd();
            }
        }

        public byte[] GetBytes(int bufferSize = 81920)
        {
            using (Stream stream = GetStream())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream, bufferSize);
                    return memoryStream.ToArray();
                }
            }
        }

        public Stream GetStream()
        {
            return HttpWebResponse.GetResponseStream();
        }

        public void Dispose()
        {
            HttpWebResponse.Dispose();
        }
    }
}
