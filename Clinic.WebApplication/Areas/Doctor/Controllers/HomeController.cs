using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
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
    }
}