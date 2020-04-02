using Clinic.Models.DomainClasses.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Utilities.ModelBinders
{
    [ModelBinder(BinderType = typeof(DoctorModelBinder))]
    public class DoctorProfilePicViewModel
    {
        public DoctorProfilePicViewModel()
        {
            Doctor = new Doctor();
            Picture = null;
            OldImagePath = null;
        }

        public DoctorProfilePicViewModel(Doctor doctor, IFormFile picture, string oldImagePath)
        {
            Doctor = doctor;
            Picture = picture;
            OldImagePath = oldImagePath;
        }

        public Doctor Doctor { get; set; }
        public IFormFile Picture { get; set; }
        public string OldImagePath { get; set; }
    }
}
