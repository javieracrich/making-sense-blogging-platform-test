using System.Collections.Generic;

namespace Domain
{
    public class Blog : BaseEntity
    {
        public virtual List<BlogPost> Posts { get; set; }
        public string Title { get; set; }
        public virtual User Author { get; set; }
    }

}
