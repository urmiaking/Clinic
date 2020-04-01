using Clinic.Models.DomainClasses.Appointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.ViewModels
{
    public class ReserveVisitViewModel
    {
        public ReserveVisitViewModel()
        {
            Visits = new List<Visit>();
            Reserves = new List<Reservation>();
        }
        public ReserveVisitViewModel(IEnumerable<Visit> visits = null, IEnumerable<Reservation> reserves = null)
        {
            Visits = visits ?? new List<Visit>();
            Reserves = reserves ?? new List<Reservation>();
        }

        public IEnumerable<Visit> Visits { get; set; }
        public IEnumerable<Reservation> Reserves { get; set; }
    }
}
