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
			return new Request().Call("Erstelle 10 Fragen für die Kategorie " + category + ", jeweils mit 4 Antwortoptionen. Jede Frage soll genau eine korrekte Antwort haben. Trenne die Fragen voneinander durch '#?#'. Trenne den Fragetext von den Antworten durch '#*#'. Trenne die einzelnen Antwortoptionen voneinander durch '~|~'. Markiere die korrekte Antwort, indem du dem Text der korrekten Antwort '[CORRECT]' voranstellst. Beispiel: 'Fragetext#*#Antwort A~|~[CORRECT]Antwort B~|~Antwort C~|~Antwort D'. Stelle sicher, dass jede Frage 4 Antwortmöglichkeiten hat.");
		}

	}
}