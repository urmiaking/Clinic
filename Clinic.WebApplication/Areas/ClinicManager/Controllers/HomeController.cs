using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
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

            ViewBag.Visits = await _db.Visits.ToListAsync();

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
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "لطفا عکس پزشک را آپلود کنید";
                return RedirectToAction("Index");
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
                return NotFound();
            }

            var doctor = await _db.Doctors
                .Include(a => a.Reservations)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (doctor == null)
            {
                return new StatusCodeResult(404);
            }

            var docWeekDay = _db.WeekDays.Where(a => a.DoctorId.Equals(id)).ToList();

            if (doctor.Reservations.Any())
            {
                return new StatusCodeResult(403);
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
            return RedirectToAction("DoctorsList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("DoctorsList");
            }

            var doctors = _db.Doctors
                .Include(a => a.Reservations)
                    .ThenInclude(a => a.Visit)
                .Where(a =>
                    a.FullName.Contains(searchQuery) ||
                    a.Specialty.Contains(searchQuery))
                .ToList();

            TempData["doctors"] = JsonConvert.SerializeObject(doctors);
            return RedirectToAction("DoctorsList");
        }


    }
}