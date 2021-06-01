using System.Threading.Tasks;

namespace Wikibot.Logic.JobRetrievers
{
    public interface IFileManager
    {
        public void WriteAllText(string text, string filePath);

        public Task<string> ReadAllTextAsync(string filePath);
    }
}