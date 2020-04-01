using Clinic.Models.DomainClasses.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.NewsPage
{
    public class News
    {
        public int Id { get; set; }

        [Display(Name = "عنوان خبر")]
        public string Title { get; set; }

        [Display(Name = "توضیح مختصر")]
        public string ShortDescription { get; set; }

        [Display(Name = "شرح خبر")]
        public string Description { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime ReleaseDate { get; set; }

        public string ImageName { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

        public int SiteAdminId { get; set; }
        public SiteAdmin SiteAdmin { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
