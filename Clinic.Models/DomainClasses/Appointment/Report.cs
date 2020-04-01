using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Report
    {
        public int Id { get; set; }

        [Display(Name = "عنوان شکایت")]
        public string Title { get; set; }

        [Display(Name = "شرح شکایت")]
        public string Description { get; set; }

        [Display(Name = "وضعیت شکایت ")]
        public string Status { get; set; }

        [Display(Name = "تاریخ شکایت")]
        public DateTime ComplainDate { get; set; }

        [Display(Name = "پاسخ شکایت")]
        public string Response { get; set; }

        public int VisitId { get; set; }

        public virtual Visit Visit { get; set; }
    }
}