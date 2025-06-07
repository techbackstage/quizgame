using PdfSharp.Fonts;
using System;
using System.IO;

namespace QuizGame.Application.Common
{
    public sealed class MyFontResolver : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // This resolver is simplified for the app's needs. It only handles Arial.
            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold && isItalic)
                    return new FontResolverInfo("Arial#BI");
                if (isBold)
                    return new FontResolverInfo("Arial#B");
                if (isItalic)
                    return new FontResolverInfo("Arial#I");
                return new FontResolverInfo("Arial");
            }
            return new FontResolverInfo(familyName);
        }

        public byte[] GetFont(string faceName)
        {
            string fontFilename = "";
            switch (faceName)
            {
                case "Arial":
                    fontFilename = "arial.ttf";
                    break;
                case "Arial#B":
                    fontFilename = "arialbd.ttf"; // Bold
                    break;
                case "Arial#I":
                    fontFilename = "ariali.ttf"; // Italic
                    break;
                case "Arial#BI":
                    fontFilename = "arialbi.ttf"; // Bold Italic
                    break;
            }

            if (!string.IsNullOrEmpty(fontFilename))
            {
                string fontPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts), fontFilename);
                if (System.IO.File.Exists(fontPath))
                {
                    return System.IO.File.ReadAllBytes(fontPath);
                }
            }
            return null!;
        }
    }
} 