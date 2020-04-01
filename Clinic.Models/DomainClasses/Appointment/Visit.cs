using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clinic.Models.DomainClasses.Appointment
{
    public class Visit
    {
        public int Id { get; set; }

        [Display(Name = "دلیل مراجعه بیمار")]
        public string CauseOfPatientReferral { get; set; }

        [Display(Name = "تشخیص پزشک")]
        public string DoctorAssessment { get; set; }

        [Display(Name = "یادداشت پزشک")]
        public string DoctorNote { get; set; }

        [Display(Name = "امتیاز داده شده")]
        public float? GivenScore { get; set; }

        public virtual Report Report { get; set; }
        public virtual Prescription Prescription { get; set; }

        public InsuranceProvider InsuranceProvider { get; set; }

        public int ReservationDoctorId { get; set; }
        public int ReservationPatientId { get; set; }
        public DateTime ReservationReserveDate { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
