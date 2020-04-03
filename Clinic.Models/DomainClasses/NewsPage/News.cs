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
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(maximumLength: 200, ErrorMessage = "حداکثر ۲۰۰ کاراکتر مجاز است")]
        public string Title { get; set; }

        [Display(Name = "توضیح مختصر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(maximumLength: 300, ErrorMessage = "حداکثر ۳۰۰ کاراکتر مجاز است")]
        public string ShortDescription { get; set; }

        [Display(Name = "شرح خبر")]
        [StringLength(maximumLength: 10000, MinimumLength = 300, ErrorMessage = "تعداد کاراکتر مجاز بین ۳۰۰ الی ۱۰۰۰۰ کاراکتر می باشد")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "عکس خبر")]
        public string ImageName { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

        public int SiteAdminId { get; set; }
        public SiteAdmin SiteAdmin { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
