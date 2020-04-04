using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Clinic.Models.DomainClasses.Users;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Drug
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام دارو")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Instruction { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "قیمت")]
        public int Cost { get; set; }

        [Display(Name = "وضعیت موجودی")]
        public bool Status { get; set; }

        [Display(Name = "دسته بندی")]
        public DrugCategory DrugCategory { get; set; }

        public ICollection<PrescriptionDrug> PrescriptionDrugs { get; set; }
    }
}