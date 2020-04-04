using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.Services.LoginService;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILoginService _loginService;

        public HomeController(AppDbContext db, ILoginService loginService)
        {
            _db = db;
            _loginService = loginService;
        }

        public async Task<IActionResult> Index()
        {
            var doctor = await _db.Doctors
                .Include(a => a.WeekDays)
                .FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            var todayReserves = await _db.Reservations
                .Include(a => a.Patient)
                .OrderBy(a => a.ReserveDate)
                .Where(a =>
                    a.Doctor.Username.Equals(doctor.Username) &&
                    a.ReserveDate.Day.Equals(DateTime.Now.Day) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"))
                .ToListAsync();

            var lastVisitedPatients = await _db.Visits
                .Include(a => a.Reservation)
                    .ThenInclude(a => a.Patient)
                .OrderByDescending(a => a.Reservation.ReserveDate)
                .Where(a => a.Reservation.DoctorId.Equals(doctor.Id))
                .Take(4)
                .ToListAsync();

            var reserveVisitVm = new ReserveVisitViewModel()
            {
                Reserves = todayReserves,
                Visits = lastVisitedPatients
            };

            var isWeekDayEmpty = doctor.WeekDays
                .All(a =>
                    a.EightTen.Equals("خالی") &&
                    a.TenTwelve.Equals("خالی") &&
                    a.TwelveFourteen.Equals("خالی") &&
                    a.FourteenSixteen.Equals("خالی"));

            ViewBag.IsWeekDayEmpty = isWeekDayEmpty;

            ViewBag.TotalVisits = await _db.Visits
                .CountAsync(a => a.Reservation.DoctorId.Equals(doctor.Id));

            ViewBag.TotalReserves = await _db.Reservations
                .CountAsync(a =>
                    a.DoctorId.Equals(doctor.Id) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت") &&
                    a.ReserveDate >= DateTime.Now);

            var visits = await _db.Visits
                .Where(a => a.Reservation.DoctorId.Equals(doctor.Id))
                .ToListAsync();

            ViewBag.MonthlyVisits = visits.Count(visit => (visit.Reservation.ReserveDate - DateTime.Now).TotalDays < 30); ;

            return View(reserveVisitVm);
        }

        [HttpPost]
        public async Task<IActionResult> AbsencePatient(string reserveDate, int patientId = 0, int docId = 0)
        {
            if (patientId == 0 || docId == 0)
            {
                return StatusCode(404);
            }

            DateTime dt = Convert.ToDateTime(reserveDate);

            var reserve = await _db.Reservations
                .FirstOrDefaultAsync(a =>
                    a.DoctorId.Equals(docId) &&
                    a.PatientId.Equals(patientId) &&
                    a.ReserveDate.Equals(dt));

            if (reserve == null)
            {
                return StatusCode(404);
            }

            reserve.ReserveStatus = "عدم حضور بیمار";

            _db.Reservations.Update(reserve);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostponeReserve(string reserveDate, string reserveTime, string oldDate, 
            int patientId = 0, int docId = 0)
        {
            if (string.IsNullOrEmpty(reserveDate) || string.IsNullOrEmpty(reserveTime))
            {
                return StatusCode(401);
            }

            DateTime dt = Convert.ToDateTime(oldDate);

            var reserve = await _db.Reservations
                .FirstOrDefaultAsync(a =>
                    a.DoctorId.Equals(docId) &&
                    a.PatientId.Equals(patientId) &&
                    a.ReserveDate.Equals(dt));

            if (reserve == null)
            {
                return StatusCode(404);
            }

            TimeSpan ts = new TimeSpan();

            if (reserveTime.StartsWith("Eight"))
            {
                ts = new TimeSpan(8, 00, 00);
            }
            if (reserveTime.StartsWith("Ten"))
            {
                ts = new TimeSpan(10, 00, 00);
            }
            if (reserveTime.StartsWith("Twelve"))
            {
                ts = new TimeSpan(12, 00, 00);
            }
            if (reserveTime.StartsWith("Fourteen"))
            {
                ts = new TimeSpan(14, 00, 00);
            }

            string[] dates = reserveDate.Split('/');
            DateTime reserveDateTime = new DateTime(Int32.Parse(dates[0]), Int32.Parse(dates[1]), Int32.Parse(dates[2]), new PersianCalendar());

            reserveDateTime = reserveDateTime.Date + ts;

            var isReservationTimeExist = await _db.Reservations.AsNoTracking()
                .AnyAsync(a =>
                    a.DoctorId.Equals(docId) &&
                    a.PatientId.Equals(patientId) &&
                    a.ReserveDate.Equals(reserveDateTime));
            
            if (isReservationTimeExist)
            {
                return StatusCode(403);
            }

            var doctor = await _db.Doctors.FindAsync(docId);
            var patient = await _db.Patients.FindAsync(patientId);

            if (doctor == null || patient == null)
            {
                return StatusCode(404);
            }

            _db.Reservations.Remove(reserve);
            await _db.SaveChangesAsync();

            var newReserve = new Reservation()
            {
                ReserveDate = reserveDateTime,
                Doctor = doctor,
                Patient = patient,
                ReserveStatus = "تعویق افتاده - در انتظار ویزیت"
            };

            await _db.Reservations.AddAsync(newReserve);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        #region TimeTable

        public async Task<IActionResult> ViewTimeTable()
        {
            var doctor = await _db.Doctors
                .FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            return View(await _db.WeekDays
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId.Equals(doctor.Id))
                .ToListAsync());
        }

        public async Task<IActionResult> EditTimeTable()
        {
            var doctor = await _db.Doctors
                .FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            var wdViewModel = new DoctorWeekDaysViewModel(await _db.WeekDays
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId.Equals(doctor.Id))
                .ToListAsync());

            return View(wdViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTimeTable(DoctorWeekDaysViewModel viewModel)
        {
            foreach (var weekDay in viewModel.WeekDays)
            {
                _db.WeekDays.Update(weekDay);
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = "برنامه روزانه ویرایش یافت";
            return RedirectToAction("ViewTimeTable");
        }

        #endregion

        #region EditProfile

        public async Task<IActionResult> EditProfile()
        { 
            var doctor = await _db.Doctors.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Models.DomainClasses.Users.Doctor doctor, IFormFile profilePic)
        {
            var oldDoctor = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(a => a.Id.Equals(doctor.Id));

            if (oldDoctor == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(doctor.Password))
            {
                if (doctor.Password.Count() < 6)
                {
                    ModelState.AddModelError("Password", "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد");
                }
                else
                {
                    doctor.Password = _loginService.GetHash(doctor.Password);
                }
            }
            else
            {
                doctor.Password = oldDoctor.Password;
            }

            doctor.Score = oldDoctor.Score;

            if (!ModelState.IsValid)
            {
                doctor.ProfilePic = oldDoctor.ProfilePic;
                ViewBag.ProfilePic = doctor.ProfilePic;
                return View(doctor);
            }

            if (!(profilePic == null || profilePic.Length == 0))
            {
                if (profilePic.Length > 0 && profilePic.Length < 500000)
                    try
                    {
                        if (!string.IsNullOrEmpty(oldDoctor.ProfilePic))
                        {
                            var oldImage = oldDoctor.ProfilePic;
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/Administrators/assets/images/doctors/", oldImage);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            else
                            {
                                TempData["Error"] = "خطا در حذف عکس";
                                return RedirectToAction("EditProfile");
                            }
                        }

                        doctor.ProfilePic = Guid.NewGuid() + Path.GetExtension(profilePic.FileName);
                        string savePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/doctors",
                            doctor.ProfilePic
                        );
                        await using var stream = new FileStream(savePath, FileMode.Create);
                        await profilePic.CopyToAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        //TODO: log error
                        Console.WriteLine(ex.Message);
                    }
                else
                {
                    TempData["Error"] = "حجم عکس بارگذاری شده برای پروفایل پزشک بیشتر از 500 کیلوبایت می باشد";
                    return RedirectToAction("EditProfile");
                }
            }

            else
            {
                doctor.ProfilePic = oldDoctor.ProfilePic;
            }

            _db.Doctors.Update(doctor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پروفایل با موفقیت ذخیره شد";
            return View(doctor);
        }

        #endregion
    }
}