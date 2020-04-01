using Clinic.Models.DomainClasses.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.ViewModels
{
    public class DoctorProfilePicViewModel
    {
        public DoctorProfilePicViewModel()
        {
            Doctor = new Doctor();
            Picture = null;
        }

        public DoctorProfilePicViewModel(Doctor doctor, IFormFile picture)
        {
            Doctor = doctor;
            Picture = picture;
        }

        public Doctor Doctor { get; set; }
        public IFormFile Picture { get; set; }
    }
}
