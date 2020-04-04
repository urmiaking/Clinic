using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.ModelValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Users
{
    public class Doctor : User
    {
        [Display(Name = "شماره نظام پزشکی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "لطفا حداکثر 10 کاراکتر وارد نمایید")]
        [DataType(DataType.PhoneNumber)]
        public string MedicalNumber { get; set; }

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
        [DataType(DataType.PhoneNumber)]
        public short Age { get; set; }

        [Display(Name = "امتیاز")]
        public float Score { get; set; }

        [Display(Name = "تخصص")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [SpecialtyCheck(ErrorMessage = "لطفا تخصص را وارد کنید")]
        public string Specialty { get; set; }

        [Display(Name = "عکس پروفایل")]
        public string ProfilePic { get; set; }

        [Display(Name = "بیوگرافی")]
        [MaxLength(1000, ErrorMessage = "لطفا حداکثر 1000 کاراکتر وارد نمایید")]
        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }

        [Display(Name = "مقالات")]
        [MaxLength(1000, ErrorMessage = "لطفا حداکثر 1000 کاراکتر وارد نمایید")]
        [DataType(DataType.MultilineText)]
        public string Articles { get; set; }

        [Display(Name = "کتاب ها و افتخارات")]
        [MaxLength(1000, ErrorMessage = "لطفا حداکثر 1000 کاراکتر وارد نمایید")]
        [DataType(DataType.MultilineText)]
        public string Books { get; set; }

        public ICollection<WeekDay> WeekDays { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
