using System;
using QuizGame;


namespace QuizGame.API
{
	public class ApiController
	{

		public static void Main(string[] args)
		{
			Console.WriteLine(Environment.getConfig("API_TOKEN"));

			new Request().Call("Gebe mir 10 Quizfragen zu der Kategorie Märchen.");

			Console.ReadLine();
		}

	}
}