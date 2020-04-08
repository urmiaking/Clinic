using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.DomainClasses.Others;
using Clinic.Services.InitService;
using Clinic.Services.LoginService;
using Clinic.Utilities.ModelBinders;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Clinic.WebApplication.Areas.ClinicManager.Controllers
{
    [Area("ClinicManager")]
    [Authorize(Roles = "ClinicManager")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILoginService _loginService;
        private readonly IInitializerService _initializerService;

        public HomeController(AppDbContext db, ILoginService loginService, IInitializerService initializerService)
        {
            _db = db;
            _loginService = loginService;
            _initializerService = initializerService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PatientCount = await _db.Patients.CountAsync();
            ViewBag.VisitCount = await _db.Visits.CountAsync();
            ViewBag.DoctorCount = await _db.Doctors.CountAsync();

            var doctors = _db.Doctors.Take(5).OrderByDescending(b => b.Score).ToList();
            DoctorListDoctorViewModel vm = new DoctorListDoctorViewModel(doctors);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(DoctorListDoctorViewModel doctorViewModel, IFormFile docImage)
        {
            if (doctorViewModel.Doctor.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد");
            }

            doctorViewModel.Doctor.Password = _loginService.GetHash(doctorViewModel.Doctor.Password);

            if (docImage != null)
            {
                if (docImage.Length > 0 && docImage.Length < 500000)
                {
                    try
                    {
                        doctorViewModel.Doctor.ProfilePic = Guid.NewGuid() + Path.GetExtension(docImage.FileName);
                        string savePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/doctors",
                            doctorViewModel.Doctor.ProfilePic
                        );
                        await using var stream = new FileStream(savePath, FileMode.Create);
                        await docImage.CopyToAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        //TODO: Log error
                        Console.WriteLine(ex.Data);
                    }
                }
                else
                {
                    TempData["Error"] = "حجم عکس بارگذاری شده برای پروفایل پزشک بیشتر از 500 کیلوبایت می باشد";
                    return RedirectToAction("DoctorsList");
                }
            }
            else
            {
                TempData["Error"] = "لطفا عکس پزشک را آپلود کنید";
                return RedirectToAction("DoctorsList");
            }

            await _db.Doctors.AddAsync(doctorViewModel.Doctor);
            await _db.SaveChangesAsync();

            await _initializerService.InitializeDoctorWeekdaysAsync(doctorViewModel.Doctor);

            TempData["Success"] = "دکتر " + doctorViewModel.Doctor.FullName + " با موفقیت ثبت نام شد";
            return RedirectToAction("DoctorsList"); //Change it to doctor list
        }

        public async Task<IActionResult> DoctorsList()
        {
            if (TempData["doctors"] != null)
            {
                var searchDocs = JsonConvert.DeserializeObject<List<Models.DomainClasses.Users.Doctor>>((string)TempData["doctors"]);
                DoctorListDoctorViewModel doctors = new DoctorListDoctorViewModel(searchDocs);
                return View(doctors);

            }
            var doctorList = await _db.Doctors.Include(a => a.Reservations)
                .ThenInclude(a => a.Doctor)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            DoctorListDoctorViewModel doctorsViewModel = new DoctorListDoctorViewModel(doctorList);
            return View(doctorsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor([ModelBinder(typeof(DoctorModelBinder))] DoctorProfilePicViewModel doctorVm)
        {
            if (doctorVm.Doctor.Password == null)
            {
                ModelState.AddModelError("Password", "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد");
            }
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.FirstOrDefault(a => a.Errors.Count > 0).Errors.FirstOrDefault()
                    .ErrorMessage;

                TempData["Error"] = message;
                return RedirectToAction("DoctorsList");
            }

            if (!(doctorVm.Picture == null || doctorVm.Picture.Length == 0))
            {
                if (doctorVm.Picture.Length > 0 && doctorVm.Picture.Length < 500000)
                    try
                    {
                        var oldImage = doctorVm.OldImagePath;
                        string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/Administrators/assets/images/doctors/", oldImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        else
                        {
                            TempData["Error"] = "خطا در حذف عکس";
                            return RedirectToAction("DoctorsList");
                        }

                        string savePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/doctors",
                            doctorVm.Doctor.ProfilePic
                        );
                        await using var stream = new FileStream(savePath, FileMode.Create);
                        await doctorVm.Picture.CopyToAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        //TODO: log error
                        Console.WriteLine(ex.Message);
                    }
                else
                {
                    TempData["Error"] = "حجم عکس بارگذاری شده برای پروفایل پزشک بیشتر از 500 کیلوبایت می باشد";
                    return RedirectToAction("DoctorsList");
                }
            }

            _db.Doctors.Update(doctorVm.Doctor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پروفایل دکتر " + doctorVm.Doctor.FullName + " با موفقیت ویرایش شد";
            return RedirectToAction("DoctorsList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var doctor = await _db.Doctors
                .Include(a => a.Reservations)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (doctor == null)
            {

                return StatusCode(404);
            }

            var docWeekDay = _db.WeekDays.Where(a => a.DoctorId.Equals(id)).ToList();

            if (doctor.Reservations.Any())
            {
                return StatusCode(403);
            }

            if (docWeekDay.Count == 0)
            {
                _db.Users.Remove(doctor);
            }
            else
            {
                foreach (var weekDay in docWeekDay)
                {
                    _db.WeekDays.Remove(weekDay);
                }
                _db.Users.Remove(doctor);
            }

            await _db.SaveChangesAsync();

            var oldImage = doctor.ProfilePic;
            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/Administrators/assets/images/doctors/", oldImage);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            else
            {
                //TODO: log error
            }

            return StatusCode(200);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("DoctorsList");
            }

            var doctors = await _db.Doctors
                .Include(a => a.Reservations)
                    .ThenInclude(a => a.Visit)
                .Where(a =>
                    a.FullName.Contains(searchQuery) ||
                    a.Specialty.Contains(searchQuery))
                .ToListAsync();

            TempData["doctors"] = JsonConvert.SerializeObject(doctors);
            return RedirectToAction("DoctorsList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string msgTitle, string msgDesc)
        {
            if (string.IsNullOrEmpty(msgTitle) || string.IsNullOrEmpty(msgDesc))
            {
                TempData["Error"] = "لطفا عنوان و شرح پیام را وارد کنید";
                return RedirectToAction("Index");
            }

            var message = new ManagerMessage()
            {
                Description = msgDesc,
                Title = msgTitle,
                Seen = false,
                DateTime = DateTime.Now
            };

            await _db.ManagerMessages.AddAsync(message);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پیغام شما با موفقیت به مدیر سایت ارسال گردید";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Complains()
        {
            var reports = await _db.Reports
                .Include(a => a.Visit)
                    .ThenInclude(a => a.Reservation)
                    .ThenInclude(a => a.Patient)
                .OrderByDescending(a => a.Status.Equals("در انتظار بررسی"))
                .ToListAsync();
            return View(reports);
        }

        public async Task<IActionResult> CheckComplain(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Complains");
            }
            var report = await _db.Reports
                .Include(a => a.Visit)
                    .ThenInclude(a => a.Reservation)
                    .ThenInclude(a => a.Patient)
                .OrderByDescending(a => a.Status.Equals("در انتظار بررسی"))
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (report == null)
            {
                return RedirectToAction("Complains");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CheckComplain(string reportResponse, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var report = await _db.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(reportResponse))
            {
                report.Status = "رد شده";
                TempData["Success"] = "شکایت رد شد";
            }
            else
            {
                report.Response = reportResponse;
                report.Status = "بررسی شده";
                TempData["Success"] = "شکایت بررسی شد";
            }

            _db.Reports.Update(report);
            await _db.SaveChangesAsync();

            return RedirectToAction("Complains");
        }

        public async Task<IActionResult> EditProfile()
        {
            string username = User.Identity.Name;

            var clinicManager = await _db.ClinicManagers.FirstOrDefaultAsync(a => a.Username.Equals(username));
            if (clinicManager == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            return View(clinicManager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(string newEmail, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var clinicManager = await _db.ClinicManagers.FindAsync(id);
            if (clinicManager == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(newEmail))
            {
                TempData["ErrorEmail"] = "لطفا ایمیل را وارد کنید!";
                return RedirectToAction("EditProfile");
            }

            clinicManager.Email = newEmail;

            _db.ClinicManagers.Update(clinicManager);
            await _db.SaveChangesAsync();

            TempData["SuccessEmail"] = "آدرس ایمیل با موفقیت تغییر یافت!";
            return RedirectToAction("EditProfile");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(string newPass, string newPassConfirm, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var clinicManager = await _db.ClinicManagers.FindAsync(id);

            if (clinicManager == null)
            {
                return NotFound();
            }

            if (newPass != newPassConfirm)
            {
                TempData["ErrorPassword"] = "رمز عبور شما با تکرار رمز عبور مطابقت ندارد!";
                return RedirectToAction("EditProfile");
            }

            if (newPass.Length < 6)
            {
                TempData["ErrorPassword"] = "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد!";
                return RedirectToAction("EditProfile");
            }

            clinicManager.Password = _loginService.GetHash(newPass);

            _db.ClinicManagers.Update(clinicManager);
            await _db.SaveChangesAsync();

            TempData["SuccessPassword"] = "رمز عبور شما با موفقیت تغییر یافت!";
            return RedirectToAction("EditProfile");
        }

        public async Task<ActionResult> InsuranceProviders()
        {
            return View(await _db.InsuranceProviders.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteInsuranceProvider(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var insurance = await _db.InsuranceProviders.FindAsync(id);

            if (insurance == null)
            {
                return StatusCode(404);
            }
            _db.InsuranceProviders.Remove(insurance);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddInsurance(string insuranceName, int discount = 0)
        {
            if (discount == 0)
            {
                TempData["Error"] = "درصد تخفیف صحیح نمی باشد";
                return RedirectToAction("InsuranceProviders");
            }

            if (!(discount > 0 && discount <= 100))
            {
                TempData["Error"] = "درصد تخفیف صحیح نمی باشد";
                return RedirectToAction("InsuranceProviders");
            }

            var insurance = new InsuranceProvider()
            {
                Discount = discount,
                InsuranceName = insuranceName
            };

            await _db.InsuranceProviders.AddAsync(insurance);
            await _db.SaveChangesAsync();

            TempData["Success"] = "شرکت بیمه با موفقیت افزوده شد";
            return RedirectToAction("InsuranceProviders");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditInsurance(string insuranceName, int discount = 0, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            if (discount == 0)
            {
                TempData["Error"] = "درصد تخفیف صحیح نمی باشد";
                return RedirectToAction("InsuranceProviders");
            }

            if (!(discount > 0 && discount <= 100))
            {
                TempData["Error"] = "درصد تخفیف صحیح نمی باشد";
                return RedirectToAction("InsuranceProviders");
            }

            var insurance = await _db.InsuranceProviders.FindAsync(id);

            if (insurance == null)
            {
                TempData["Error"] = "شرکت بیمه یافت نشد";
                return RedirectToAction("InsuranceProviders");
            }

            insurance.InsuranceName = insuranceName;
            insurance.Discount = discount;

            _db.InsuranceProviders.Update(insurance);
            await _db.SaveChangesAsync();

            TempData["Success"] = "بیمه " + insuranceName + " با موفقیت ویرایش شد";
            return RedirectToAction("InsuranceProviders");
        }
    }
}