using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Extensions.Configuration;

namespace Wikibot.App.Jobs
{
    public class TestJob : AbstractJob
    {
        public TestJob(IConfiguration config) {
            Configuration = config;
        }

        public void Execute() {

            SetJobStart();
            try
            {
                Console.WriteLine("Test Job writing to console.");
            }
            catch(Exception ex)
            {
                Comment = $"An error occurred: {ex.StackTrace}";
            }
            finally
            {
                SetJobEnd();
                SaveJob();
            }
        }
    }
}