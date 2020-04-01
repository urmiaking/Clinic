using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class PrescriptionDrug
    {
        [Key, Column(Order = 0)]
        public int PrescriptionId { get; set; }

        [Key, Column(Order = 1)]
        public int DrugId { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "وضعیت خرید")]
        public bool IsBought { get; set; }

        [Display(Name = "تمایل به خرید")]
        public bool IsWantToBuy { get; set; }

        public Prescription Prescription { get; set; }
        public Drug Drug { get; set; }
    }
}