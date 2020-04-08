using Clinic.Models.DomainClasses.NewsPage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Models.DomainClasses.Users
{
    public abstract class User
    {
        public int Id { get; set; }

        [Display(Name = "ایمیل")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "لطفا حداکثر 100 کاراکتر وارد نمایید")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل فرمت صحیحی ندارد")]
        [Remote(areaName: "", action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "لطفا حداکثر 50 کاراکتر وارد نمایید")]
        [Remote(areaName:"", action: "IsUsernameInUse", controller: "Account")]
        public string Username { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "نام کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "لطفا حداکثر 100 کاراکتر وارد نمایید")]
        public string FullName { get; set; }

        public string UserType { get; set; }

        public string ResetPasswordCode { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}
