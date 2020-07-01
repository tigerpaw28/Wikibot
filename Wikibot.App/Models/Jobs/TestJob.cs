using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;

namespace Wikibot.App.Jobs
{
    public class TestJob : WikiJob
    {
        public void Execute() {
            Console.WriteLine("Test Job writing to console.");
        }
    }
}