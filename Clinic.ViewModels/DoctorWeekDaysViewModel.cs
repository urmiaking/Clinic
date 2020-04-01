using Clinic.Models.DomainClasses.Appointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.ViewModels
{
    public class DoctorWeekDaysViewModel
    {
        public DoctorWeekDaysViewModel()
        {
            WeekDays = new List<WeekDay>();
        }
        public DoctorWeekDaysViewModel(List<WeekDay> weekDay = null)
        {
            WeekDays = weekDay ?? new List<WeekDay>();
        }

        public List<WeekDay> WeekDays { get; set; }
    }
}
