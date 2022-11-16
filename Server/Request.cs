using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.Responses;

namespace Server
{
    public static class Request
    {
        public static async Task<Response> CreateResponse(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var reqString = req.Url.AbsolutePath[1..];
            if (!Enum.TryParse<RequestType>(reqString, true, out var requestType))
            {
                Logger.Log($"Unknown request: {reqString}", Logger.Level.Error);
                return null;
            }

            var reqText = req.QueryString["data"];
            await Logger.LogAsync(reqText);
            var json = string.IsNullOrEmpty(reqText) ? null : NiceJson.JsonNode.ParseJsonString(reqText);
            return requestType switch
            {
                RequestType.SignUp => new SignupResponse(json),
                RequestType.SignIn => new SigninResponse(json),
                RequestType.GetProfile => new GetProfileResponse(json),
                RequestType.CreateDesk => new CreateDeskResponse(json),
                RequestType.AddCard => new CreateCardResponse(json),
                RequestType.GetCards => new GetAllCardsResponse(json),
                RequestType.DeleteCard => new DeleteCardResponse(json),
                RequestType.MoveCard => new MoveCardResponse(json),
                RequestType.GetCardsByTag => new GetCardsByTagResponse(json),
                RequestType.Test => new TestResponse(),
                _ => null
            };
        }

        public enum RequestType
        {
            SignUp,
            SignIn,
            GetProfile,
            CreateDesk,
            AddCard,
            GetCards,
            GetCardsByTag,
            DeleteCard,
            MoveCard,
            Test
        }
    }
}
