using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Others
{
    public class ManagerMessage
    {
        public int Id { get; set; }

        [Display(Name = "عنوان پیام")]
        public string Title { get; set; }

        [Display(Name = "شرح پیام")]
        public string Description { get; set; }

        [Display(Name = "تاریخ پیام")]
        public DateTime DateTime { get; set; }

        [Display(Name = "وضعیت پیام")]
        public bool Seen { get; set; }
    }
}
