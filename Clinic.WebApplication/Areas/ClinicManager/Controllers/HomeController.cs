using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.ClinicManager.Controllers
{
    [Area("ClinicManager")]
    [Authorize(Roles = "ClinicManager")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PatientCount = await _db.Patients.CountAsync();
            ViewBag.VisitCount = await _db.Visits.CountAsync();
            ViewBag.DoctorCount = await _db.Doctors.CountAsync();

            ViewBag.Visits = await _db.Visits.ToListAsync();
            ViewBag.Reports = await _db.Reports.Where(a => a.Status.Equals("در انتظار بررسی")).ToListAsync();

            var doctors = _db.Doctors.Take(5).OrderByDescending(b => b.Score).ToList();
            DoctorListDoctorViewModel vm = new DoctorListDoctorViewModel(doctors);

            return View(vm);
        }
    }
}