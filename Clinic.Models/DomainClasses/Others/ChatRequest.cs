using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models.DomainClasses.Users;

namespace Clinic.Models.DomainClasses.Others
{
    public class ChatRequest
    {
        public int Id { get; set; }
        public string PatientUsername { get; set; }
        public Doctor Doctor { get; set; }
    }
}
