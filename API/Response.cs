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


        public Response(HttpResponseMessage? response)
        {
            readResponse(response);
            JObject jsonObject = JObject.Parse(this.content);
            this.content = jsonObject["choices"][0]["message"]["content"].ToString();

            new Parser(this.content).parse();
        }

        protected async void readResponse(HttpResponseMessage? response)
        {
            this.content = await response.Content.ReadAsStringAsync();
        }

        public string getContent()
        {
            return this.content;
        }
    }
}