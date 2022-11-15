using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NiceJson;

namespace Server
{
    public abstract class Response
    {
        protected JsonNode request;
        public virtual bool CanGiveToken { get; }
        public bool CanRecieveToken { get; }

        public Response(JsonNode req)
        {
            request = req;
        }

        public abstract Task<string> Process();
        public virtual string GetToken() => string.Empty;
        public virtual void SetToken(string token) { }
    }
}
