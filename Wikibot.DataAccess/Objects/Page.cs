using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Wikibot.DataAccess.Objects
{
    public class Page
    {
        public Page() { }
        public Page(long id, string name)
        {
            PageID = id;
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PageID { get; set; }

        public string Name { get; set; }
    }
}
