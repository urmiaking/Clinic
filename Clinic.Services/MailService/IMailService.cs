using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Services.MailService
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(string emailAddress, string body, string subject);
    }
}
