using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace QuizGame
{
    public class Environment
    {

        public static dynamic getProjectRoot() => AppContext.BaseDirectory;

       
        public static string getConfig(string option)
        {
            string json = File.ReadAllText(Path.Combine(Environment.getProjectRoot(), "config.json"));
            Dictionary<string, JsonElement> config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            return config.ContainsKey (option) ? config[option].ToString() : "NO_VALUE";
        }
    }

    public class SomeClass
    {
        private static readonly string Bareer = Environment.getConfig("API_TOKEN") ?? string.Empty;

        // Rest of the class implementation
    }
}

