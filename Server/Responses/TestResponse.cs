using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json;
using NiceJson;

namespace Server.Responses
{
    public class TestResponse : Response
    {
        public TestResponse() : base(null) { }

        public override async Task<string> Process()
        {
            await Task.CompletedTask;
            return JsonUtil.OK;
        }
    }
}