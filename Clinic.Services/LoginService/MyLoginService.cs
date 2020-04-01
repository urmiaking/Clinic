using Clinic.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Users;
using Clinic.Services.MailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Clinic.Services.LoginService
{
    public class MyLoginService : ILoginService
    {
        private readonly AppDbContext _db;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyLoginService(AppDbContext appDbContext, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _db = appDbContext;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateCookieAsync(User user, bool rememberMe = false)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(43200)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), authProperties);

            return true;
        }

        public string GetHash(string password)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sb = new StringBuilder();

            foreach (var t in result)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        public async Task<bool> SendActivationLinkAsync(string userEmail)
        {
            string activationCode = Guid.NewGuid().ToString();

            var link = string.Concat(
                _httpContextAccessor.HttpContext.Request.Scheme,
                "://",
                _httpContextAccessor.HttpContext.Request.Host.ToUriComponent(),
                $"/Account/ActivateAccount/{activationCode}");

            var getUser = await _db.Patients.FirstOrDefaultAsync(a => a.Email.Equals(userEmail));
            if (getUser == null)
            {
                return false;
            }

            getUser.ActivationCode = activationCode;
            await _db.SaveChangesAsync();

            const string subject = "فعال سازی حساب کاربری | کلینیک تخصصی بهار";
            var body = "سلام " + getUser.FullName + ", <br/> لطفا برای فعالسازی حساب کاربری خود در وب سایت کلینیک فوق تخصصی بهار، روی لینک زیر کلیک کنید. " +

                       " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                       "اگر شما در این وبسایت حسابی باز نکردید لطفا این پیغام را نادیده بگیرید.<br/><br/> با تشکر";

            var isSucceed = await _mailService.SendEmailAsync(getUser.Email, body, subject);

            return isSucceed;
        }

        public async Task<bool> SendResetPasswordLinkAsync(string userEmail)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Email.Equals(userEmail));
            if (user == null)
            {
                return false;
            }
            string resetCode = Guid.NewGuid().ToString();
            var link = string.Concat(
                _httpContextAccessor.HttpContext.Request.Scheme,
                "://",
                _httpContextAccessor.HttpContext.Request.Host.ToUriComponent(),
                $"/Account/ResetPassword/{resetCode}");

            user.ResetPasswordCode = resetCode;
            await _db.SaveChangesAsync();

            const string subject = "درخواست فراموشی رمز عبور | کلینیک تخصصی بهار";
            var body = "سلام " + user.FullName + ", <br/> شما اخیرا درخواست تغییر رمز عبور خود را در صفحه ورود به سیستم کلینیک فوق تخصصی بهار نموده اید. لطفا روی لینک زیر کلیک کنید و رمز جدید خود را وارد نمایید. " +

                       " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                       "اگر شما درخواست تغییر رمز عبور را نداده اید لطفا این ایمیل را نادیده بگیرید<br/><br/> با تشکر";

            await _mailService.SendEmailAsync(user.Email, body, subject);
            return true;
        }

        public async Task<User> SignInUserAsync(LoginViewModel user)
        {
            var hashPassword = GetHash(password: user.Password);

            return await _db.Users.FirstOrDefaultAsync(a =>
                a.Password.Equals(hashPassword) &&
                a.Username.Equals(user.Username));
        }
    }
}
