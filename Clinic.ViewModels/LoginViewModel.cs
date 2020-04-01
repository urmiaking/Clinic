using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            Username = "";
            Password = "";
            RememberMe = false;
        }

        public LoginViewModel(string username = "", string password = "", bool rememberMe = false)
        {
            Username = username;
            Password = password;
            RememberMe = rememberMe;
        }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "نام کاربری")]
        [MaxLength(50, ErrorMessage = "لطفا حداکثر 50 کاراکتر وارد نمایید")]
        public string Username { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "مرا بخاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
