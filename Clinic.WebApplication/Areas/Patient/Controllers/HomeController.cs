using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var patient = await _db.Patients.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            var visits = await _db.Visits
                .Include(a => a.Reservation)
                .ThenInclude(a => a.Doctor)
                .Where(a => a.Reservation.PatientId.Equals(patient.Id)).ToListAsync();

            ViewBag.TotalVisitsCount = visits.Count;
            ViewBag.TotalReservesCount = await _db.Reservations
                .Where(a =>
                    a.PatientId.Equals(patient.Id) && a.ReserveStatus.Contains("در انتظار ویزیت"))
                .CountAsync();

            ViewBag.MonthlyVisitsCount =
                visits.Count(visit => (visit.Reservation.ReserveDate - DateTime.Now).TotalDays < 30);

            var reserves = await _db.Reservations.Include(a => a.Doctor)
                .Where(a =>
                    a.PatientId.Equals(patient.Id) &&
                    a.ReserveDate.Day.Equals(DateTime.Now.Day) &&
                    a.ReserveStatus.Equals("در انتظار ویزیت"))
                .ToListAsync();

            var reserveVisitViewModel = new ReserveVisitViewModel()
            {
                Reserves = reserves,
                Visits = visits
            };

            return View(reserveVisitViewModel);
        }

        #region Reserve

        public IActionResult Reserve()
        {
            ReserveDoctorsViewModel vm = new ReserveDoctorsViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(ReserveDoctorsViewModel reserveDoctors)
        {
            if (!ModelState.IsValid)
            {
                return View(reserveDoctors);
            }

            if (!TimeSpan.TryParse(reserveDoctors.Reserve.ReserveTime, out var time))
            {
                ModelState.AddModelError("Reserve.ReserveTime", "ساعت فرمت مناسبی ندارد");
                return View(reserveDoctors);
            }

            var dates = reserveDoctors.Reserve.ReserveDate.Split('/');
            var reserveDateTime = new DateTime(
                int.Parse(dates[0]),
                int.Parse(dates[1]),
                int.Parse(dates[2]), new PersianCalendar());
            reserveDateTime = reserveDateTime.Date + time;

            if (DateTime.Now > reserveDateTime)
            {
                TempData["Error"] = "تاریخ وارد شده باید در آینده باشد.";
                return View(reserveDoctors);
            }

            var dayOfWeek = reserveDateTime.DayOfWeek switch
            {
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنج شنبه",
                DayOfWeek.Saturday => "شنبه",
                _ => null
            };

            if (string.IsNullOrEmpty(dayOfWeek))
            {
                TempData["Error"] = "امکان رزرو در روز جمعه وجود ندارد";
                return View(reserveDoctors);
            }

            var weekdays = await _db.WeekDays
                .Include(a => a.Doctor)
                .Where(a => (a.DayName.Equals(dayOfWeek)) &&
                            a.Doctor.Specialty.Equals(reserveDoctors.Reserve.DoctorSpecialty))
                .ToListAsync();

            var patient = await _db.Patients
                .FirstOrDefaultAsync(a =>
                    a.Username.Equals(User.Identity.Name));

            foreach (var day in weekdays)
            {
                var hasAlreadyThatReserveWithChosenDoctorOnThatDay = await _db.Reservations
                    .AnyAsync(a =>
                        a.ReserveDate.Day.Equals(reserveDateTime.Day) &&
                        a.Doctor.Id.Equals(day.Doctor.Id) &&
                        a.Patient.Id.Equals(patient.Id) &&
                        a.ReserveStatus.Equals("در انتظار ویزیت"));

                var hasReserveOnThatTime = await _db.Reservations
                    .AnyAsync(a =>
                        a.Patient.Id.Equals(patient.Id) &&
                        a.ReserveDate.Equals(reserveDateTime) &&
                        a.ReserveStatus.Equals("در انتظار ویزیت"));

                var hasFourReserveWithThatDoctor = await _db.Reservations
                                                       .CountAsync(a =>
                                                           a.DoctorId.Equals(day.Doctor.Id) &&
                                                           a.PatientId.Equals(patient.Id) &&
                                                           a.ReserveStatus.Equals("در انتظار ویزیت")) >= 4;

                var hasReachedReserveLimit = await _db.Reservations
                                                 .CountAsync(a =>
                                                     a.PatientId.Equals(patient.Id) &&
                                                     a.ReserveStatus.Equals("در انتظار ویزیت")) >= 20;

                if (!(hasReserveOnThatTime || hasAlreadyThatReserveWithChosenDoctorOnThatDay
                                           || hasFourReserveWithThatDoctor
                                           || hasReachedReserveLimit))
                {
                    switch (time.Hours)
                    {
                        case 8 when day.EightTen.Equals("خالی"):
                            reserveDoctors.Doctors.Add(day.Doctor);
                            break;
                        case 10 when day.TenTwelve.Equals("خالی"):
                            reserveDoctors.Doctors.Add(day.Doctor);
                            break;
                        case 12 when day.TwelveFourteen.Equals("خالی"):
                            reserveDoctors.Doctors.Add(day.Doctor);
                            break;
                        case 14 when day.FourteenSixteen.Equals("خالی"):
                            reserveDoctors.Doctors.Add(day.Doctor);
                            break;
                    }
                }
            }

            ViewBag.ReserveTime = reserveDateTime;
            ViewBag.ReserveDate = reserveDoctors.Reserve.ReserveDate;

            return View(reserveDoctors);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReserve(string reserveDateTime, int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var candidateReserveDateTime = Convert.ToDateTime(reserveDateTime);

            if (candidateReserveDateTime.Equals(DateTime.MinValue))
            {
                return StatusCode(404);
            }

            var doctor = await _db.Doctors.FindAsync(id);
            var patient = await _db.Patients.FirstOrDefaultAsync(a =>
                a.Username.Equals(User.Identity.Name));

            if (doctor == null)
            {
                return StatusCode(404);
            }

            var isCandidateReserveDateTimeBusy = await _db.Reservations
                .AnyAsync(a =>
                    a.ReserveDate.Equals(candidateReserveDateTime) &&
                    a.Doctor.Id.Equals(doctor.Id) &&
                    a.ReserveStatus.Equals("در انتظار ویزیت"));

            var isDailyReserveLimitExceeded = await _db.Reservations
                .AnyAsync(a =>
                    a.ReserveDate.Day.Equals(candidateReserveDateTime.Day) &&
                    a.Doctor.Id.Equals(doctor.Id) &&
                    a.Patient.Id.Equals(patient.Id) &&
                    a.ReserveStatus.Equals("در انتظار ویزیت"));

            if (isCandidateReserveDateTimeBusy)
            {
                return StatusCode(401);
            }

            if (isDailyReserveLimitExceeded)
            {
                return StatusCode(402);
            }

            var reserve = new Reservation()
            {
                Doctor = doctor,
                Patient = patient,
                ReserveStatus = "در انتظار ویزیت",
                ReserveDate = candidateReserveDateTime
            };
            await _db.Reservations.AddAsync(reserve);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReserve(string reserveDate, int docId = 0, int patientId = 0)
        {
            if (docId == 0 || patientId == 0)
            {
                return StatusCode(404);
            }

            var reserveDateTime = Convert.ToDateTime(reserveDate);

            var reserve = await _db.Reservations
                .FirstOrDefaultAsync(a =>
                    a.DoctorId.Equals(docId) &&
                    a.PatientId.Equals(patientId) &&
                    a.ReserveDate.Equals(reserveDateTime));

            if (reserve == null)
            {
                return StatusCode(404);
            }

            _db.Reservations.Remove(reserve);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        public async Task<IActionResult> ReserveList()
        {
            var reserveList = await _db.Reservations
                .Include(a => a.Doctor)
                .Where(a =>
                    a.Patient.Username.Equals(User.Identity.Name) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"))
                .ToListAsync();

            return View(reserveList);
        }

        #endregion

        #region Visit
        public async Task<IActionResult> VisitList()
        {
            var visitList = await _db.Visits
                .Include(a => a.Reservation)
                .ThenInclude(a => a.Doctor)
                .Where(a => a.Reservation.Patient.Username.Equals(User.Identity.Name))
                .ToListAsync();

            return View(visitList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int rate = 0, int visitId = 0)
        {
            if (rate == 0)
            {
                return NotFound();
            }

            var visit = await _db.Visits
                .Include(a => a.Reservation)
                .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id.Equals(visitId));

            if (visit == null)
            {
                return NotFound();
            }

            visit.GivenScore = rate;

            _db.Visits.Update(visit); 
            await _db.SaveChangesAsync();

            var doctor = visit.Reservation.Doctor;

            if (doctor == null)
            {
                return NotFound();
            }

            var scores = await _db.Visits
                .Where(a => 
                    a.GivenScore.HasValue && 
                    a.Reservation.Doctor.Id.Equals(doctor.Id))
                .Select(a => a.GivenScore)
                .ToListAsync();

            var average = scores.Average();

            if (average != null)
            {
                doctor.Score = average.Value;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "امتیاز شما با موفقیت ثبت شد";
            return RedirectToAction("VisitList", "Home");
        }


        #endregion
    }
}