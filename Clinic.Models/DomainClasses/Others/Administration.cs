using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Models.DomainClasses.Others
{
    public class Administration
    {
        public int Id { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        public int Port { get; set; }

        public string Host { get; set; }
    }
}
