using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "رمز عبور جدید")]
        [Required(ErrorMessage = "لطفا رمز عبور جدید خود را وارد کنید", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "لطفا حداکثر ۶ کاراکتر وارد کنید")]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار رمز عبور جدید")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "رمز جدید با رمز وارد شده مطابقت ندارد")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "کد فعالسازی موجود نیست")]
        public string ResetCode { get; set; }
    }
}
