using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.DomainClasses.Users;

namespace Clinic.ViewModels
{
    public class ReserveDoctorsViewModel
    {
        public ReserveDoctorsViewModel(List<Doctor> doctors, ReserveViewModel reserve)
        {
            Doctors = doctors;
            Reserve = reserve;
        }

        public ReserveDoctorsViewModel()
        {
            Doctors = new List<Doctor>();
            Reserve = new ReserveViewModel();
        }
        public List<Doctor> Doctors { get; set; }
        public ReserveViewModel Reserve { get; set; }
    }
}
