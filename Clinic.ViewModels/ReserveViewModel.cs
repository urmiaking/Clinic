using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Clinic.ViewModels
{
    public class ReserveViewModel
    {
        [Display(Name = "تاریخ ملاقات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ReserveDate { get; set; }

        [Display(Name = "ساعت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ReserveTime { get; set; }

        [Display(Name = "تخصص پزشک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DoctorSpecialty { get; set; }
    }
}
