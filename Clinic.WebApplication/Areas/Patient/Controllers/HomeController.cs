using System;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
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

            ViewBag.MonthlyVisitsCount = visits.Count(visit => (visit.Reservation.ReserveDate - DateTime.Now).TotalDays < 30);

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
    }
}