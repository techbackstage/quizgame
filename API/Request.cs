using QuizGame;
using QuizGame.API;
using System;
using System.Diagnostics;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.API
{
    public class Request
    {
        private static readonly string Bareer = Environment.getConfig("API_TOKEN");
        
        private static readonly string bash = "curl -X POST \\\r\n\t\thttps://llm.chutes.ai/v1/chat/completions \\\r\n\t\t-H \"Authorization: Bearer $CHUTES_API_TOKEN\" \\\r\n\t-H \"Content-Type: application/json\" \\\r\n\t-d '  {\r\n    \"model\": \"deepseek-ai/DeepSeek-V3-0324\",\r\n    \"messages\": [\r\n      {\r\n        \"role\": \"user\",\r\n        \"content\": \"$CHUTES_REQUEST_MESSAGE\"\r\n      }\r\n    ],\r\n    \"stream\": false,\r\n    \"max_tokens\": 1024,\r\n    \"temperature\": 0.7\r\n  }'";


        public string Call(string message)
        {
            string request = bash.Replace("$CHUTES_API_TOKEN", Request.Bareer);
            request = bash.Replace("$CHUTES_REQUEST_MESSAGE", message);

            Request.CallBash(message);

            return "";
        }

        private static async void CallBash(string message)
        {
            string apiToken = Request.Bareer;
            string requestMessage = message;

            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://llm.chutes.ai/v1/chat/completions");

            request.Headers.Add("Authorization", $"Bearer {apiToken}");
            request.Headers.Add("Accept", "application/json");

            var jsonBody = $@"
        {{
            ""model"": ""deepseek-ai/DeepSeek-V3-0324"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": ""{EscapeForJson(requestMessage)}""
                }}
            ],
            ""stream"": false,
            ""max_tokens"": 1024,
            ""temperature"": 0.7
        }}";

            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
          
            new Response(response);
        }

        private static string EscapeForJson(string input)
        {
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}