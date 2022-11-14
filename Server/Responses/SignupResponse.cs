using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Server.Responses
{
    public class SignupResponse : Response
    {
        private const string command = "INSERT INTO Users (username, password, salt, name) VALUES (:username, :password, :salt, :name)";

        private string username, password, name, salt;

        private bool process = true;

        public SignupResponse(NameValueCollection query, HttpListenerResponse resp) : base(query, resp)
        {
            try
            {
                username = query["username"];
                password = Hash.HashString64(query["password"]);
                salt = new Random().Next(65536).ToString("X2");
                name = query["name"];
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
            var com = Server.Users.CreateCommand(command);
            com.Parameters.AddWithValue("username", username);
            com.Parameters.AddWithValue("password", password);
            com.Parameters.AddWithValue("salt", salt);
            com.Parameters.AddWithValue("name", name);
            await com.ExecuteNonQueryAsync();
        }
    }
}