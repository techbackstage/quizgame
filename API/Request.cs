using QuizGame;
using QuizGame.API;
using System;
using System.Diagnostics;

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

            Request.CallBash(request);

            return "";
        }

        private static void CallBash(string request)
        {
            // Prozess erstellen, um den Bash-Befehl auszuführen
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Git\bin\bash.exe",  // Pfad zur Git Bash
                    Arguments = $"-c \"{request}\"",  // Der Befehl als Argument an bash übergeben
                    RedirectStandardOutput = true,  // Standardausgabe umleiten
                    RedirectStandardError = true,  // Standardfehler umleiten
                    UseShellExecute = false,  // Muss auf false gesetzt werden, um Ausgabe zu bekommen
                    CreateNoWindow = true  // Verhindert, dass ein neues Terminal-Fenster geöffnet wird
                }
            };

            process.Start();

            // Ausgabe und Fehler lesen
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();  // Warten, bis der Befehl beendet ist

            // Ausgabe anzeigen
            Console.WriteLine("OUTPUT:");
            Console.WriteLine(output);

            // Fehler anzeigen, falls vorhanden
            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(error);
            }
        }
    }
}