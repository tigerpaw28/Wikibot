using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using WikiFunctions;

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

        public static void GenerateAndSaveDiff(string beforeContent, string afterContent, string title, long id, string directory, string folderName)
        {
            beforeContent = beforeContent.Replace("\n", "\r\n");
            afterContent = afterContent.Replace("\n", "\r\n");
            var wikiDiff = new WikiDiff();
            string diff = $"{WikiDiff.DiffHead()}</head><body>{WikiDiff.TableHeader}{wikiDiff.GetDiff(beforeContent, afterContent, 1)}</table></body></html>";
            string filename = "Diff-" + id + "-" + title + ".txt"; //Set filename for this page
            filename = SanitizeFilename(filename, '_');

            string filePath = Path.Combine(directory, folderName, filename);
            File.WriteAllText(filePath, diff);
        }
    }
}
