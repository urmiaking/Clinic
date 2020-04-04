using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize(Roles = "Pharmacy")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var drugs = await _db.Drugs.ToListAsync();

            var prescriptions = await _db.Prescriptions
                    .Include(a => a.PrescriptionDrugs)
                    .Include(a => a.Visit)
                        .ThenInclude(a => a.Reservation)
                            .ThenInclude(a => a.Patient)
                    .Include(a => a.Visit)
                        .ThenInclude(a => a.InsuranceProvider)
                    .Where(a =>
                        a.Status.Equals("پرداخت نشده") ||
                        a.PaymentMethod.Equals("غیر نقدی")).ToListAsync();

            ViewBag.AllPrescriptions = await _db.Prescriptions.CountAsync();
            ViewBag.AllDeliveredPrescriptions = await _db.Prescriptions.CountAsync(a => a.Status.Equals("تحویل داده شده"));

            var prescriptionDrugVm = new PharmacyDrugPrescriptionViewModel()
            {
                Drugs = drugs,
                Prescriptions = prescriptions
            };

            return View(prescriptionDrugVm);
        }
    }
}