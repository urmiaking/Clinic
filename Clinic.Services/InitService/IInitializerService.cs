using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Clinic.Models.DomainClasses.Users;

namespace Clinic.Services.InitService
{
    public interface IInitializerService
    {
        Task<bool> InitializeDoctorWeekdaysAsync(Doctor doctor);
    }
}
