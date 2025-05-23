using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;


namespace QuizGame.API
{
    public class Response
    {
        private string content = "";


        public Response(HttpResponseMessage response)
        {
            readResponse(response);
            JObject jsonObject = JObject.Parse(this.content);
            
            var contentToken = jsonObject["choices"]?[0]?["message"]?["content"];
            if (contentToken != null)
            {
                this.content = contentToken.ToString();
            }
            else
            {
                this.content = "";
            }
        }

        protected async void readResponse(HttpResponseMessage? response)
        {
            if (response != null)
            {
                this.content = await response.Content.ReadAsStringAsync();
            }
        }

        public string getContent()
        {
            return this.content;
        }
    }
}