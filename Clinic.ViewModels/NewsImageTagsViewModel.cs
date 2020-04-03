using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Clinic.Models.DomainClasses.NewsPage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Clinic.ViewModels
{
    public class NewsImageTagsViewModel
    {
        public NewsImageTagsViewModel()
        {
            News = new News();
            File = null;
            Tags = null;
        }
        public NewsImageTagsViewModel(News news, IFormFile file, string tags)
        {
            News = news;
            File = file;
            Tags = tags;
        }
        public News News { get; set; }

        [Required(ErrorMessage = "لطفا عکسی را برای خبر آپلود کنید")]
        public IFormFile File { get; set; }

        [Display(Name = "برچسب ها")]
        [Required(ErrorMessage = "لطفا حداقل یک برچسب برای خبر وارد کنید")]
        public string Tags { get; set; }
    }
}
