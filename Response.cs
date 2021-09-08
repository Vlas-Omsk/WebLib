using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using PinkJson;

namespace WebLib
{
    public class Response
    {
        public string Content { get; internal set; }
        public WebHeaderCollection Headers => WebResponse.Headers;
        public bool IsHaveErrors { get; internal set; }
        public HttpStatusCode StatusCode => WebResponse.StatusCode;

        public HttpWebResponse WebResponse { get; internal set; }

        public Json GetJson()
        {
            return new Json(Content);
        }
    }
}
