using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Clinic.Models.DomainClasses.Appointment;

namespace Clinic.ViewModels
{
    public class AddVisitViewModel
    {
        public Visit Visit { get; set; }
        public Reservation Reservation { get; set; }

        [Display(Name = "داروها")]
        public string DrugList { get; set; }

        [Display(Name = "تاریخ ملاقات")]
        public string ReserveDateAgain { get; set; }

        [Display(Name = "ساعت")]
        public string ReserveTimeAgain { get; set; }

        [Display(Name = "نوع بیمه")]
        public string InsuranceProviderName { get; set; }

        public AddVisitViewModel(Visit visit, Reservation reservation, string drugList,
            string reserveDateAgain, string reserveTimeAgain, string insuranceProviderName)
        {
            Visit = visit ?? new Visit();
            DrugList = drugList;
            ReserveDateAgain = reserveDateAgain;
            ReserveTimeAgain = reserveTimeAgain;
            InsuranceProviderName = insuranceProviderName;
            Reservation = reservation ?? new Reservation();
        }

        public AddVisitViewModel()
        {
            Visit = new Visit();
            DrugList = null;
            ReserveDateAgain = null;
            ReserveTimeAgain = null;
            InsuranceProviderName = null;
        }
    }
}
