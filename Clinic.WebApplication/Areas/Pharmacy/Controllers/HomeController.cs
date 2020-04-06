using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.Services.LoginService;
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
        private readonly ILoginService _loginService;

        public HomeController(AppDbContext db, ILoginService loginService)
        {
            _db = db;
            _loginService = loginService;
        }

        public async Task<IActionResult> Index()
        {
            var drugs = await _db.Drugs
                .Include(a => a.DrugCategory)
                .OrderByDescending(a => a.Id)
                .ToListAsync();

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

        #region PrescriptionCheck

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrescriptionCheck(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var prescription = await _db.Prescriptions
                .Include(a => a.PrescriptionDrugs)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));
            
            if (prescription == null)
            {
                return NotFound();
            }

            var presDrug = await _db.PrescriptionDrugs
                .Include(a => a.Drug)
                .Where(a => a.Prescription.Id.Equals(id))
                .ToListAsync();

            foreach (var item in presDrug)
            {
                item.Drug.Count -= item.Count;
                if (item.Drug.Count >= 0) continue;
                TempData["Error"] = "داروهای داروخانه کافی نمی باشند";
                return RedirectToAction("Index");
            }
            prescription.Status = "تحویل داده شده";
            _db.Prescriptions.Update(prescription);
            await _db.SaveChangesAsync();

            TempData["Success"] = "دارو با موفقیت تحویل داده شد";
            return RedirectToAction("Index");
        }

        #endregion

        #region EditProfile

        public async Task<IActionResult> EditProfile()
        {
            return View(await _db.Users.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(string newEmail, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var pharmacy = await _db.Pharmacies.FindAsync(id);

            if (pharmacy == null)
            {
                return NotFound();
            }
            pharmacy.Email = newEmail;

            _db.Pharmacies.Update(pharmacy);
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

            var pharmacy = await _db.Pharmacies.FindAsync(id);

            if (pharmacy == null)
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

            pharmacy.Password = _loginService.GetHash(newPass);

            _db.Pharmacies.Update(pharmacy);
            await _db.SaveChangesAsync();

            TempData["SuccessPassword"] = "رمز عبور شما با موفقیت تغییر یافت!";
            return RedirectToAction("EditProfile");
        }

        #endregion

        #region PrescriptionList

        public async Task<IActionResult> PrescriptionList()
        {
            var prescriptions = await _db.Prescriptions
                .Include(a => a.PrescriptionDrugs)
                .ThenInclude(a => a.Drug)
                .Include(a => a.Visit)
                    .ThenInclude(a => a.Reservation)
                        .ThenInclude(a => a.Patient)
                .Include(a => a.Visit)
                    .ThenInclude(a => a.InsuranceProvider)
                .Where(a => a.PaymentMethod.Equals("نقدی"))
                .ToListAsync();

            return View(prescriptions);
        }

        public async Task<IActionResult> PrescriptionOnlineList()
        {
            var prescriptions = await _db.Prescriptions
                .Include(a => a.PrescriptionDrugs)
                .Include(a => a.Visit)
                    .ThenInclude(a => a.Reservation)
                        .ThenInclude(a => a.Patient)
                .Include(a => a.Visit)
                    .ThenInclude(a => a.InsuranceProvider)
                .Where(a => a.PaymentMethod.Equals("غیر نقدی"))
                .ToListAsync();

            return View(prescriptions);
        }

        #endregion

        #region Drugs

        public async Task<IActionResult> ManageDrugs()
        {
            return View(await _db.Drugs.Include(a => a.DrugCategory).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDrugs(string drugName, string drugCategory, string drugDescription, int drugCount,
            int drugCost = 0, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var drug = await _db.Drugs.FindAsync(id);

            if (drug == null)
            {
                return NotFound();
            }

            var drugCategoryItem = await _db.DrugCategories.FirstOrDefaultAsync(a => a.Name.Equals(drugCategory));

            if (drugCategoryItem == null)
            {
                TempData["Error"] = "دسته بندی دارو نامعتبر است";
                return RedirectToAction("ManageDrugsCategory");
            }

            drug.Name = drugName;
            drug.DrugCategory = drugCategoryItem;
            drug.Cost = drugCost;
            drug.Count = drugCount;
            drug.Instruction = drugDescription;

            _db.Drugs.Update(drug);
            await _db.SaveChangesAsync();

            TempData["Success"] = "داروی " + drug.Name + " با موفقیت ویرایش داده شد ";
            return RedirectToAction("ManageDrugs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDrug(string drugName, string drugCategory, string drugDescription, int drugCount,
            int drugCost = 0)
        {
            var pharmacy = await _db.Pharmacies.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));
            if (pharmacy == null)
            {
                return NotFound();
            }
            var drugCategoryItem = await _db.DrugCategories.FirstOrDefaultAsync(a => a.Name.Equals(drugCategory));

            if (drugCategoryItem == null)
            {
                TempData["Error"] = "دسته بندی دارو نامعتبر است";
                return RedirectToAction("ManageDrugsCategory");
            }

            var drug = new Drug()
            {
                Name = drugName,
                DrugCategory = drugCategoryItem,
                Instruction = drugDescription,
                Count = drugCount,
                Cost = drugCost
            };
            await _db.Drugs.AddAsync(drug);
            await _db.SaveChangesAsync();

            TempData["Success"] = "داروی " + drug.Name + " با موفقیت اضافه شد ";
            return RedirectToAction("ManageDrugs");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDrug(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var drug = await _db.Drugs.FindAsync(id);

            if (drug == null)
            {
                return StatusCode(404);
            }

            if (await _db.PrescriptionDrugs.AnyAsync(a => a.DrugId.Equals(drug.Id)))
            {
                return StatusCode(403);
            }

            _db.Drugs.Remove(drug);
            await _db.SaveChangesAsync();

            return StatusCode(200);

        }
        #endregion

        #region DrugCategory

        public async Task<IActionResult> ManageDrugsCategory()
        {
            return View(await _db.DrugCategories.OrderBy(a => a.Name).ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDrugCategory(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var category = await _db.DrugCategories
                .Include(a => a.Drugs)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (category == null)
            {
                return StatusCode(404);
            }

            var drugs = await _db.Drugs.Where(a => a.DrugCategory.Id.Equals(category.Id)).ToListAsync();

            if ((await _db.PrescriptionDrugs.ToListAsync()).Any(prescriptionDrug => drugs.Any(drug => prescriptionDrug.DrugId.Equals(drug.Id))))
            {
                return StatusCode(403);
            }

            _db.Drugs.RemoveRange(drugs);
            _db.DrugCategories.Remove(category);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDrugCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                TempData["Error"] = "لطفا نام دسته را وارد نمایید";
                return RedirectToAction("ManageDrugsCategory");
            }
            var category = new DrugCategory()
            {
                Name = categoryName
            };

            await _db.DrugCategories.AddAsync(category);
            await _db.SaveChangesAsync();

            TempData["Success"] = "دسته جدید با موفقیت افزوده شد";
            return RedirectToAction("ManageDrugsCategory");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDrugCategory(string categoryName, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                TempData["Error"] = "لطفا نام دسته را وارد نمایید";
                return RedirectToAction("ManageDrugsCategory");
            }

            var category = await _db.DrugCategories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "دسته یافت نشد";
                return RedirectToAction("ManageDrugsCategory");
            }

            category.Name = categoryName;

            _db.DrugCategories.Update(category);
            await _db.SaveChangesAsync();

            TempData["Success"] = "دسته " + category.Name + " با موفقیت ویرایش شد";
            return RedirectToAction("ManageDrugsCategory");
        }

        #endregion
    }
}