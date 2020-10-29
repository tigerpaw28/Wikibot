using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
