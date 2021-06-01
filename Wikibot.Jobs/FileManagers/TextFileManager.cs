using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wikibot.Logic.JobRetrievers;

namespace Wikibot.Logic.FileManagers
{
    public class TextFileManager : IFileManager
    {
        public void WriteAllText(string text, string filePath)
        {

        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
