using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class InsuranceCard
    {
        public int Id { get; set; }

        [Display(Name = "نام شرکت بیمه کننده")]
        public string InsuranceName { get; set; }

        [Display(Name = "تخفیف")]
        public int Discount { get; set; }

        public ICollection<Visit> Visits { get; set; }

    }
}