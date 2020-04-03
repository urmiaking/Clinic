using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Clinic.Models.DomainClasses.NewsPage;
using Microsoft.AspNetCore.Http;

namespace Clinic.ViewModels
{
    public class NewsImageTagsEditViewModel
    {
        public NewsImageTagsEditViewModel()
        {
            News = new News();
            File = null;
        }
        public NewsImageTagsEditViewModel(News news, IFormFile file)
        {
            News = news;
            File = file;
        }
        public News News { get; set; }

        public IFormFile File { get; set; }
    }
}
