using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class DrugCategory
    {
        public int Id { get; set; }
        [Display(Name = "نام دسته بندی")]
        public string Name { get; set; }
        public ICollection<Drug> Drugs { get; set; }
    }
}
