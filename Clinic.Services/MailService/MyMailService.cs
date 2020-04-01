using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Clinic.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Services.MailService
{
    public class MyMailService : IMailService
    {
        private readonly AppDbContext _db;

        public MyMailService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> SendEmailAsync(string emailAddress, string body, string subject)
        {
            var server = await _db.Administrations.FirstOrDefaultAsync(a => a.Port.Equals(587));
            if (server == null)
            {
                return false;
            }
            using (MailMessage mm = new MailMessage(server.Email, emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = server.Host;
                smtp.EnableSsl = true;

                NetworkCredential networkCredential = new NetworkCredential(server.Email, server.Password);
                smtp.Credentials = networkCredential;
                smtp.Port = server.Port;
                smtp.Send(mm);
            }
            _db.Dispose();
            return true;
        }
    }
}
