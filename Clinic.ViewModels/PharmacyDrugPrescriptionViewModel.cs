using Clinic.Models.DomainClasses.Appointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.ViewModels
{
    public class PharmacyDrugPrescriptionViewModel
    {
        public PharmacyDrugPrescriptionViewModel()
        {
            Drugs = new List<Drug>();
            Prescriptions = new List<Prescription>();
        }

        public PharmacyDrugPrescriptionViewModel(IEnumerable<Drug> drugs = null,
            IEnumerable<Prescription> prescriptions = null)
        {
            Drugs = drugs ?? new List<Drug>();
            Prescriptions = prescriptions ?? new List<Prescription>();
        }

        public IEnumerable<Drug> Drugs { get; set; }
        public IEnumerable<Prescription> Prescriptions { get; set; }
    }
}
