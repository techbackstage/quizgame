using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace QuizGame
{
    public class Environment
    {

        public static string getProjectRoot() => AppContext.BaseDirectory;

       
        public static string getConfig(string option)
        {
            string json = File.ReadAllText(Path.Combine(getProjectRoot(), "config.json"));
            var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (config != null && config.TryGetValue(option, out var value))
            {
                return value.ToString();
            }
            
            return "NO_VALUE";
        }
    }
}

