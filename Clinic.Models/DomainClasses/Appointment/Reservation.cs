using Clinic.Models.DomainClasses.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Reservation
    {
        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        [Display(Name = "تاریخ رزرو")]
        public DateTime ReserveDate { get; set; }

        [Display(Name = "وضعیت رزرو")]
        public string ReserveStatus { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public virtual Visit Visit { get; set; }
    }
}