using System;
using System.Collections.Generic;
using System.Windows;
using QuizGame.Application.Model;


namespace QuizGame.API
{
	public class ApiController
	{

		public static List<Question> Run(string category)
		{
			return new Request().Call("Gib mir 10 Fragen mit 4 Antwortoptionen zur Kategorie " + category + " wovon nur eine richtig sein soll. Trenne Fragen mit #?#, die Frage von den Antworten mit '#*#' und die einzelnen Antworten mit '#-#' außer bei der richtigen Antwort da verwende #+#, die Trennzeichen sollen links und rechts vom Inhalt stehen.");
		}

	}
}