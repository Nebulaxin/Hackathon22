using System.Threading.Tasks;
using NiceJson;

namespace Server
{
    public abstract class Response
    {
        protected JsonNode request;

        public Response(JsonNode req)
        {
            request = req;
        }

        public abstract Task<string> Process();
    }
}
