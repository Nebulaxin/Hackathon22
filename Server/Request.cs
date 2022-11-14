using System;
using System.Net;
using System.Text.RegularExpressions;
using Server.Responses;

namespace Server
{
    public class Request
    {
        private HttpListenerRequest request;
        private RequestType requestType;
        public Request(HttpListenerRequest req)
        {
            request = req;
            if (!Enum.TryParse<RequestType>(request.Url.AbsolutePath[1..], true, out requestType))
                Logger.Log("Unknown request", Logger.Level.Error);
        }

        public Response CreateResponse(HttpListenerResponse resp)
        {
            var q = request.QueryString;
            return requestType switch
            {
                RequestType.SignUp => new SignupResponse(q, resp),
                _ => throw null
            };
        }

        public enum RequestType
        {
            SignUp,
            SignIn,
        }
    }
}
