using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Wikibot.Logic
{
    public class Utilities
    {
        private static List<char> _BadChars;
        // replaces invalid characters with replaceChar
        public static string SanitizeFilename(string filename, char replaceChar)
        {
            // construct a list of characters that can't show up in filenames.
            // need to do this because ":" is not in InvalidPathChars
            if (_BadChars == null)
            {
                _BadChars = new List<char>(Path.GetInvalidFileNameChars());
            }

            foreach (char c in _BadChars)
            {
                filename = filename.Replace(c, replaceChar);
            }

            return filename;
        }
    }
}
