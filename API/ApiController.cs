using System;
using System.Windows;
using application;
using QuizGame;


namespace QuizGame.API
{
	public class ApiController
	{

		public static void Run()
		{
			//Console.WriteLine(Environment.getConfig("API_TOKEN"));

			//new Request().Call("Gib mir zwei Fragen mit 2 Antwortoptionen zur Kategorie Spiele wovon nur eine richtig sein soll. Trenne Fragen mit #?#, die Frage von den Antworten mit '#*#' und die einzelnen Antworten mit '#-#' au�er debi der richtigen antwort da verwende #+#, die trennzeichen sollen links und rechts vom Inhalt stehen.");

			// Console.ReadLine();
			
			try
			{
				var main = new MainWindow();
				main.Show();
			}
			catch (System.Exception ex)
			{
				MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

	}
}