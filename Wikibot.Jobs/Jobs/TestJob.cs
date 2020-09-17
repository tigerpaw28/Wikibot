using Microsoft.Extensions.Configuration;
using System;

namespace Wikibot.Logic.Jobs
{
    public class TestJob : WikiJob
    {
        public TestJob(IConfiguration config) {
            Configuration = config;
        }

        public override void Execute() {

            SetJobStart();
            try
            {
                Console.WriteLine("Test Job writing to console.");
            }
            catch(Exception ex)
            {
               Console.WriteLine($"An error occurred: {ex.StackTrace}");
            }
            finally
            {
                SetJobEnd();
                SaveRequest();
            }
        }
    }
}