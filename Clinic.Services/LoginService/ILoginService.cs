using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Clinic.Models.DomainClasses.Users;
using Clinic.ViewModels;

namespace Clinic.Services.LoginService
{
    public interface ILoginService
    {
        Task<User> SignInUserAsync(LoginViewModel user);

        string GetHash(string password);

        Task<bool> SendActivationLinkAsync(string userEmail);

        Task<bool> SendResetPasswordLinkAsync(string userEmail);

        Task<bool> CreateCookieAsync(User user, bool rememberMe = false);
    }
}
