using Clinic.Models.DomainClasses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.ViewModels
{
    public class DoctorListDoctorViewModel
    {
        public DoctorListDoctorViewModel()
        {
            Doctor = new Doctor();
            Doctors = new List<Doctor>();
        }
        public Doctor Doctor { get; set; }
        public List<Doctor> Doctors { get; set; }

        public DoctorListDoctorViewModel(List<Doctor> doctors = null, Doctor doctor = null)
        {
            Doctor = doctor ?? new Doctor();
            Doctors = doctors ?? new List<Doctor>();
        }
    }
}
