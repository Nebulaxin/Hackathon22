using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Server.Responses
{
    public class SigninResponse : Response
    {
        private const string command = "INSERT INTO Users (username, password, salt, name) VALUES (:username, :password, :salt, :name)";

        private string username, password;

        private bool process = true;

        public SigninResponse(NameValueCollection query, HttpListenerResponse resp) : base(query, resp)
        {
            try
            {
                username = query["username"];
                password = Hash.HashString64(query["password"]);
            }
            catch
            {
                process = false;
                resp.StatusCode = 400;
            }
        }

        public override async Task Process()
        {

            if (!process) return;
            var com = Server.Users.CreateCommand("");
            await com.ExecuteNonQueryAsync();
        }
    }
}