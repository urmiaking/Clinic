﻿using System;
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
        }
        public NewsImageTagsViewModel(News news, IFormFile file)
        {
            News = news;
            File = file;
        }
        public News News { get; set; }

        [Required(ErrorMessage = "لطفا عکسی را برای خبر آپلود کنید")]
        public IFormFile File { get; set; }
    }
}
