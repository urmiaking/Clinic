using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Models.DomainClasses.NewsPage
{
    public class Tag
    {
        public Tag()
        {
            this.News = new HashSet<News>();
        }

        public int Id { get; set; }

        public string TagValue { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
