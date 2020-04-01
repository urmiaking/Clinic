using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Models.DomainClasses.NewsPage
{
    public class Tag
    {
        public int Id { get; set; }

        public string TagValue { get; set; }

        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
