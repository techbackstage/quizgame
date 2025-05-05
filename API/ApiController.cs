using System;
using QuizGame;


namespace QuizGame.API
{
	public class ApiController
	{

		public static void Main(string[] args)
		{
			Console.WriteLine(Environment.getConfig("API_TOKEN"));

			new Request().Call("Gib mir eine Frage mit 2 Antwortoptionen wovon nur eine richtig sein soll. Trenne die Frage von den Antworten mit '###' und die einzelnen Antworten mit '#?#'.");

			Console.ReadLine();
		}

	}
}