using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.ModelValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Users
{
    public class Patient : User
    {
        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "لطفا حداکثر 10 کاراکتر وارد نمایید")]
        [DataType(DataType.PhoneNumber)]
        public string NationalCode { get; set; }

        [Display(Name = "شماره همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شما مجاز به وارد کردن 11 کاراکتر می باشید.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [GenderCheck(ErrorMessage = "لطفا جنسیت را وارد کنید")]
        public string Gender { get; set; }

        [Display(Name = "سن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [ValidateAge(ErrorMessage = "سن عددی بین ۱۸ تا ۱۵۰ می باشد")]
        [Range(1, 150, ErrorMessage = "لطفا عددی مجاز وارد کنید")]
        public short Age { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(400, ErrorMessage = "لطفا حداکثر 400 کاراکتر وارد نمایید")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "کد پستی")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        [Display(Name = "عکس بیمار")]
        public string ProfilePic { get; set; }

        public string ActivationCode { get; set; }

        public bool IsValidated { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
