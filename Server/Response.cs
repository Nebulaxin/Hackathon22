using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Response
    {
        protected HttpListenerResponse response;
        protected NameValueCollection query;
        private HttpListenerResponse resp;

        public Response(NameValueCollection query, HttpListenerResponse resp)
        {
            response = resp;
            this.query = query;
        }

        protected Response(HttpListenerResponse resp)
        {
            this.resp = resp;
        }

        public abstract Task Process();
    }
}
