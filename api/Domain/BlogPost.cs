using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BlogPost : BaseEntity
    {
        public virtual User Author { get; set; }
        [MaxLength(5000)]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public BlogPostStatus Status { get; set; }
        public virtual Blog Blog { get; set; }

    }

    public enum BlogPostStatus
    {
        draft = 1,
        @private = 2,
        @public = 3
    }

}
