using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace QuizGame.Application.Services
{
    public class PdfExportService
    {
        public void CreatePdf(List<Category> categoriesToExport, string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Quiz-Kategorien Export";

            XFont fontTitle = new XFont("Arial", 20, XFontStyleEx.Bold);
            XFont fontCategory = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont fontQuestion = new XFont("Arial", 10, XFontStyleEx.Regular);
            XFont fontCorrectAnswer = new XFont("Arial", 8, XFontStyleEx.Bold);
            XFont fontAnswer = new XFont("Arial", 10, XFontStyleEx.Regular);

            double yPosition = 40;
            const double xMargin = 40;
            const double pageHeight = 842;
            const double lineHeight = 18;
            const double paragraphSpacing = 10;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            void CheckAndCreateNewPage()
            {
                if (yPosition > pageHeight - (2 * xMargin))
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = xMargin;
                }
            }

            gfx.DrawString("Exportierte Quiz-Kategorien", fontTitle, XBrushes.Black, xMargin, yPosition);
            yPosition += fontTitle.GetHeight() + paragraphSpacing * 2;

            using (var db = QuizDbContext.getContext())
            {
                foreach (var category in categoriesToExport)
                {
                    CheckAndCreateNewPage();
                    gfx.DrawString($"Kategorie: {category.Name}", fontCategory, XBrushes.DarkBlue, xMargin, yPosition);
                    yPosition += fontCategory.GetHeight() + paragraphSpacing;

                    var dbCategory = db.Categories.Include(c => c.Questions).ThenInclude(q => q.Answers).FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (dbCategory == null || !dbCategory.Questions.Any())
                    {
                        CheckAndCreateNewPage();
                        gfx.DrawString("  Keine Fragen in dieser Kategorie.", fontAnswer, XBrushes.Gray, xMargin + 10, yPosition);
                        yPosition += lineHeight;
                        continue;
                    }

                    foreach (var question in dbCategory.Questions)
                    {
                        CheckAndCreateNewPage();
                        gfx.DrawString($"Frage: {question.Text}", fontQuestion, XBrushes.Black, xMargin + 10, yPosition);
                        yPosition += fontQuestion.GetHeight() + paragraphSpacing / 2;

                        if (!question.Answers.Any())
                        {
                            CheckAndCreateNewPage();
                            gfx.DrawString("    Keine Antworten für diese Frage.", fontAnswer, XBrushes.Gray, xMargin + 20, yPosition);
                            yPosition += lineHeight;
                            continue;
                        }

                        foreach (var answer in question.Answers.OrderBy(a => a.AnswerOptionId))
                        {
                            CheckAndCreateNewPage();
                            XFont currentAnswerFont = answer.IsCorrect ? fontCorrectAnswer : fontAnswer;
                            XBrush currentAnswerBrush = answer.IsCorrect ? XBrushes.Green : XBrushes.Black;
                            string prefix = answer.IsCorrect ? "Richtige Antwort: " : "Antwort: ";

                            gfx.DrawString($"{prefix}{answer.Text}", currentAnswerFont, currentAnswerBrush, xMargin + 20, yPosition);
                            yPosition += currentAnswerFont.GetHeight();
                        }
                        yPosition += paragraphSpacing;
                    }
                    yPosition += paragraphSpacing * 1.5;
                }
            }
            document.Save(filePath);
        }
    }
} 