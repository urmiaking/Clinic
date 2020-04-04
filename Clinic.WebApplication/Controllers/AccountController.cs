using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Users;
using Clinic.Services.LoginService;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILoginService _loginService;

        public AccountController(AppDbContext db, ILoginService loginService)
        {
            _db = db;
            _loginService = loginService;
        }
        #region Login
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("SiteAdmin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "SiteAdmin" });
                }
                if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Doctor" });
                }
                if (User.IsInRole("Patient"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Patient" });
                }
                if (User.IsInRole("ClinicManager"))
                {
                    return RedirectToAction("Index", "Home", new { area = "ClinicManager" });
                }
                if (User.IsInRole("Pharmacy"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Pharmacy" });
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = WebUtility.UrlDecode(returnUrl);

            var validUser = await _loginService.SignInUserAsync(user);

            if (validUser != null)
            {
                await _loginService.CreateCookieAsync(validUser, user.RememberMe);

                if (Url.IsLocalUrl(decodedUrl))
                {
                    return Redirect(decodedUrl);
                }

                switch (validUser)
                {
                    case Patient patient when patient.IsValidated:
                        return RedirectToAction("Index", "Home", new { area = "Patient" });
                    case Patient patient:
                        ViewBag.Error = "اکانت غیر فعال است";
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return View();
                    case ClinicManager _:
                        return RedirectToAction("Index", "Home", new { area = "ClinicManager" });
                    case SiteAdmin _:
                        return RedirectToAction("Index", "Home", new { area = "SiteAdmin" });
                    case Pharmacy _:
                        return RedirectToAction("Index", "Home", new { area = "Pharmacy" });
                    case Doctor _:
                        return RedirectToAction("Index", "Home", new { area = "Doctor" });
                }
            }

            ViewBag.Error = "نام کاربری یا رمز عبور شما اشتباه است";
            return View();
        }
        #endregion

        #region Register

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Patient patient)
        {
            patient.Username = patient.Username.Trim();

            var existUsername = await _db.Users.FirstOrDefaultAsync(a => a.Username.Equals(patient.Username));

            var existNationalCode =
                await _db.Patients.FirstOrDefaultAsync(a => a.NationalCode.Equals(patient.NationalCode));

            var existEmail = await _db.Users.FirstOrDefaultAsync(a => a.Email.Equals(patient.Email));

            patient.ProfilePic = "avatat-11.png";

            if (existUsername != null)
            {
                ModelState.AddModelError("Username", "کاربری با این نام کاربری موجود می باشد");
            }

            if (existNationalCode != null)
            {
                ModelState.AddModelError("NationalCode", "کاربری با این کد ملی موجود می باشد");
            }

            if (existEmail != null)
            {
                ModelState.AddModelError("Email", "کاربری با این ایمیل موجود می باشد");
            }

            if (string.IsNullOrEmpty(patient.Password))
            {
                ModelState.AddModelError("Password", "لطفا رمز عبور را وارد کنید");
            }

            if (patient.Password.Count() < 6)
            {
                ModelState.AddModelError("Password", "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد");
            }
            else
            {
                patient.Password = _loginService.GetHash(patient.Password);
            }

            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            await _db.Patients.AddAsync(patient);
            await _db.SaveChangesAsync();

            var isSucceed = await _loginService.SendActivationLinkAsync(patient.Email);

            if (!isSucceed)
            {
                TempData["Error"] =
                    "مشکلی در ارسال لینک فعالسازی پیش آمد، لطفا با مدیر کلینیک تماس بگیرید.";
                return RedirectToAction("Login", "Account");
            }

            TempData["Success"] =
                "ثبت نام با موفقیت انجام شد. لطفا برای تکمیل مراحل ثبت نام لینک فعالسازی ارسال شده به ایمیلتان را چک کنید";

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ActivateAccount(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var patient = await _db.Patients.FirstOrDefaultAsync(a => a.ActivationCode.Equals(id));
            if (patient == null)
            {
                return NotFound();
            }

            patient.IsValidated = true;
            patient.ActivationCode = null;
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();

            TempData["Success"] = "حساب شما با موفقیت فعال شد";
            await _loginService.CreateCookieAsync(patient);
            return RedirectToAction("Index", "Home", new { area = "Patient" });
        }

        #endregion

        #region ResetPassword

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                ViewBag.Error = "لطفا آدرس ایمیل را وارد نمایید";
                return View();
            }

            var isSucceed = await _loginService.SendResetPasswordLinkAsync(userEmail);
            if (!isSucceed)
            {
                ViewBag.Error = "کاربری با این آدرس ایمیل وجود ندارد";
                return View();
            }
            ViewBag.Success = "!لینک فراموشی رمز عبور به ایمیل شما ارسال شد";
            return View();
        }


        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _db.Users.FirstOrDefaultAsync(a => a.ResetPasswordCode.Equals(id));
            if (user == null)
            {
                return NotFound();
            }

            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel()
            {
                ResetCode = id
            };

            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _db.Users.FirstOrDefaultAsync(a => a.ResetPasswordCode == model.ResetCode);

            if (user != null)
            {
                var hashedPassword = _loginService.GetHash(model.NewPassword);
                user.Password = hashedPassword;
                user.ResetPasswordCode = null;
                await _db.SaveChangesAsync();

                ViewBag.Success = "رمز عبور شما با موفقیت تغییر یافت";
                return View(model);
            }

            ViewBag.Error = "کد نامعتبر است";
            return View(model);
        }

        #endregion

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}