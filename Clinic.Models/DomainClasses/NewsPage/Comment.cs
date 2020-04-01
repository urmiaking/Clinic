using Clinic.Models.DomainClasses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Models.DomainClasses.NewsPage
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public bool IsConfirmed { get; set; }

        public News News { get; set; }

        public User User { get; set; }

        public DateTime DateTime { get; set; }

        public ICollection<Reply> Replies { get; set; }
    }
}
