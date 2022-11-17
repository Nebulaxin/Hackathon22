using System.Threading.Tasks;

namespace Server.Responses
{
    public class TestResponse : Response
    {
        public TestResponse() : base(null) { }

        public override async Task<string> Process()
        {
            await Task.CompletedTask;
            return Util.OK;
        }
    }
}
