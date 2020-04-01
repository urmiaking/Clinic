using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Prescription
    {
        public int Id { get; set; }

        [Display(Name = "وضعیت نسخه")]
        public string Status { get; set; }

        [Display(Name = "روش پرداخت")]
        public string PaymentMethod { get; set; }


        public ICollection<PrescriptionDrug> PrescriptionDrugs { get; set; }

        public virtual Visit Visit { get; set; }
    }
}