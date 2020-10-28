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
                //_BadChars.Union(Path.GetInvalidPathChars());
            }

            //// remove root
            //string root = Path.GetPathRoot(path);
            //path = path.Remove(0, root.Length);

            //// split on the directory separator character. Need to do this
            //// because the separator is not valid in a filename.
            //List<string> parts = new List<string>(path.Split(new char[] { Path.DirectorySeparatorChar }));

            //// check each part to make sure it is valid.
            //for (int i = 0; i < parts.Count; i++)
            //{
            //    string part = parts[i];
                foreach (char c in _BadChars)
                {
                    filename = filename.Replace(c, replaceChar);
                }
                //parts[i] = part;
            //}

            return filename; //root + Utility.Join(parts, Path.DirectorySeparatorChar.ToString());
        }
    }
}
