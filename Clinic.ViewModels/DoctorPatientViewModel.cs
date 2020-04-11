using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models.DomainClasses.Users;

namespace Clinic.ViewModels
{
    public class DoctorPatientViewModel
    {
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }

        public DoctorPatientViewModel(Doctor doctor, Patient patient)
        {
            Doctor = doctor;
            Patient = patient;
        }

        public DoctorPatientViewModel()
        {
            Doctor = new Doctor();
            Patient = new Patient();
        }
    }
}
