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
			return new Request().Call("Gib mir zwei Fragen mit 10 Antwortoptionen zur Kategorie " + category + " wovon nur eine richtig sein soll. Trenne Fragen mit #?#, die Frage von den Antworten mit '#*#' und die einzelnen Antworten mit '#-#' au�er debi der richtigen antwort da verwende #+#, die trennzeichen sollen links und rechts vom Inhalt stehen.");
		}

	}
}