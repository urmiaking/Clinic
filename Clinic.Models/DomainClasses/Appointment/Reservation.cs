using Clinic.Models.DomainClasses.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Reservation
    {
        [Key, Column(Order = 0)]
        public int PatientId { get; set; }

        [Key, Column(Order = 1)]
        public int DoctorId { get; set; }

        [Key, Column(Order = 2)]
        [Display(Name = "تاریخ رزرو")]
        public DateTime ReserveDate { get; set; }

        [Display(Name = "وضعیت رزرو")]
        public string ReserveStatus { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public virtual Visit Visit { get; set; }
    }
}