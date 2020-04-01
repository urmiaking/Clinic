using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Services.MailService;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Services.FeedBackService
{
    public class MyFeedBackService: IFeedBackService
    {
        private readonly IMailService _mailService;
        private readonly AppDbContext _db;

        public MyFeedBackService(IMailService mailService, AppDbContext db)
        {
            _mailService = mailService;
            _db = db;
        }

        public async Task<bool> SendFeedBackAsync(string name, string email, string message)
        {
            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            bool hasTags = tagRegex.IsMatch(message);

            if (hasTags)
            {
                return false;
            }

            var subject = "نظر از " + name + " با آدرس ایمیل " + email + " | کلینیک تخصصی بهار";
            var managerEmail = _db.Users.FirstOrDefault(a => a.UserType.Equals("ClinicManager"))?.Email;

            var isSucceed = await _mailService.SendEmailAsync(managerEmail, message, subject);

            return isSucceed;
        }
    }
}
