using Clinic.Models.DomainClasses.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class WeekDay
    {
        public int Id { get; set; }

        [Display(Name = "روز هفته")]
        public string DayName { get; set; }
        public string EightTen { get; set; }
        public string TenTwelve { get; set; }
        public string TwelveFourteen { get; set; }
        public string FourteenSixteen { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
