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
            readResponseSync(response);
            
            try
            {
                if (string.IsNullOrWhiteSpace(this.content))
                {
                    Console.WriteLine("Empty response content");
                    this.content = "";
                    return;
                }

                // Log the raw response for debugging
                Console.WriteLine($"Raw API Response: {this.content}");
                
                JObject jsonObject = JObject.Parse(this.content);
                
                var contentToken = jsonObject["choices"]?[0]?["message"]?["content"];
                if (contentToken != null)
                {
                    this.content = contentToken.ToString();
                    Console.WriteLine($"Extracted content: {this.content}");
                }
                else
                {
                    Console.WriteLine("No content token found in response");
                    this.content = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                Console.WriteLine($"Raw content: {this.content}");
                this.content = "";
            }
        }

        protected void readResponseSync(HttpResponseMessage? response)
        {
            if (response != null)
            {
                this.content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }

        public string getContent()
        {
            return this.content;
        }
    }
}