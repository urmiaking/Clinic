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
using Microsoft.Extensions.Logging;

namespace Clinic.WebApplication.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILoginService _loginService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext db, ILoginService loginService, ILogger<HomeController> logger)
        {
            _db = db;
            _loginService = loginService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var patient = await _db.Patients.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            var visits = await _db.Visits
                .Include(a => a.Prescription)
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
                    a.Patient.Username.Equals(User.Identity.Name) &&
                    a.ReserveDate.Hour >= DateTime.Now.Hour &&
                    a.ReserveDate.Day == DateTime.Now.Day &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"))
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

            var isCandidateReserveDateTimeBusyDoctor = await _db.Reservations
                .AnyAsync(a =>
                    a.ReserveDate.Equals(candidateReserveDateTime) &&
                    a.Doctor.Id.Equals(doctor.Id) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"));

            var isDailyReserveLimitExceeded = await _db.Reservations
                .AnyAsync(a =>
                    a.ReserveDate.Day.Equals(candidateReserveDateTime.Day) &&
                    a.Doctor.Id.Equals(doctor.Id) &&
                    a.Patient.Id.Equals(patient.Id) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"));

            var isCandidateReserveDateTimeBusyPatient = await _db.Reservations
                .AnyAsync(a =>
                    a.ReserveDate.Equals(candidateReserveDateTime) &&
                    a.PatientId.Equals(patient.Id) &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"));

            if (isCandidateReserveDateTimeBusyDoctor)
            {
                return StatusCode(401);
            }

            if (isCandidateReserveDateTimeBusyPatient)
            {
                return StatusCode(403);
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
                    a.ReserveDate.Date >= DateTime.Now.Date &&
                    a.ReserveStatus.Contains("در انتظار ویزیت"))
                .ToListAsync();

            return View(reserveList);
        }

        #endregion

        #region Visit
        public async Task<IActionResult> VisitList()
        {
            var visitList = await _db.Visits
                .Include(a => a.Prescription)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeComplain(string reportTitle, string reportDesc, int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var visit = await _db.Visits.FindAsync(id);

            if (visit == null)
            {
                return NotFound();
            }

            var newReport = new Report()
            {
                ComplainDate = DateTime.Now,
                Description = reportDesc,
                Title = reportTitle,
                Visit = visit,
                Status = "در انتظار بررسی"
            };

            await _db.Reports.AddAsync(newReport);
            await _db.SaveChangesAsync();

            TempData["Success"] = "شکایت شما با موفقیت ثبت شد";
            return RedirectToAction("PrescriptionDetails", new { id });

        }

        #endregion

        #region Prescription

        public async Task<IActionResult> PrescriptionDetails(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var visit = await _db.Visits
                .Include(a => a.Prescription)
                .Include(a => a.Reservation)
                .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (visit == null)
            {
                return NotFound();
            }

            var prescriptionDrug = await _db.PrescriptionDrugs
                .Include(a => a.Prescription)
                .ThenInclude(a => a.Visit)
                .ThenInclude(a => a.Report)
                .Include(a => a.Drug)
                .Where(a => a.Prescription.VisitId.Equals(visit.Id))
                .ToListAsync();

            ViewBag.DocAssessment = visit.DoctorAssessment;
            ViewBag.PatientReferral = visit.CauseOfPatientReferral;
            ViewBag.DocNote = visit.DoctorNote;

            return View(prescriptionDrug);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bill(List<PrescriptionDrug> prescriptionDrugs)
        {
            foreach (var prescriptionDrug in prescriptionDrugs)
            {
                _db.PrescriptionDrugs.Update(prescriptionDrug);
                await _db.SaveChangesAsync();
            }

            var isThereAnyWantedList = prescriptionDrugs.Any(a => a.IsWantToBuy.Equals(true));

            if (!isThereAnyWantedList)
            {
                TempData["Error"] = "لطفا حداقل یک دارو را انتخاب کنید";

                return RedirectToAction("PrescriptionDetails",
                    new { id = prescriptionDrugs.FirstOrDefault()?.PrescriptionId });
            }

            var prescriptionId = prescriptionDrugs.FirstOrDefault()?.PrescriptionId;
            var postedPrescriptionDrugs = await _db.PrescriptionDrugs
                .Include(a => a.Drug)
                .ThenInclude(a => a.DrugCategory)
                .Include(a => a.Prescription)
                    .ThenInclude(a => a.Visit)
                        .ThenInclude(a => a.Reservation)
                            .ThenInclude(a => a.Patient)
                .Include(a => a.Prescription)
                    .ThenInclude(a => a.Visit)
                        .ThenInclude(a => a.InsuranceProvider)
                .Where(a =>
                    a.PrescriptionId.Equals(prescriptionId) &&
                    a.IsWantToBuy.Equals(true))
                .ToListAsync();

            if (postedPrescriptionDrugs.Any(a => a.Drug.DrugCategory.Name.Equals("نامشخص")))
            {
                TempData["Error"] = "امکان پرداخت آنلاین بدلیل نامشخص بودن یکی از داروهای شما وجود ندارد";

                return RedirectToAction("PrescriptionDetails",
                    new { id = prescriptionDrugs.FirstOrDefault()?.PrescriptionId });
            }

            return View(postedPrescriptionDrugs);
        }

        public async Task<IActionResult> OnlinePayment(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var prescription = await _db.Prescriptions
                .Include(a => a.PrescriptionDrugs)
                .Include(a => a.Visit)
                .ThenInclude(a => a.InsuranceProvider)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (prescription == null)
            {
                return NotFound();
            }

            var drugs = await _db.PrescriptionDrugs
                .Include(a => a.Prescription)
                .ThenInclude(a => a.Visit)
                .ThenInclude(a => a.InsuranceProvider)
                .Include(a => a.Drug)
                .Where(a =>
                    a.PrescriptionId.Equals(prescription.Id) &&
                    a.IsWantToBuy.Equals(true))
                .ToListAsync();

            var discount = 0;
            var totalCost = drugs.Sum(drug => drug.Drug.Cost * drug.Count);

            if (prescription.Visit.InsuranceProvider != null)
            {
                discount += drugs.Sum(drug => drug.Drug.Cost * drug.Count * (prescription.Visit.InsuranceProvider.Discount) / 100);

                totalCost -= discount;
            }

            ViewBag.TotalCost = totalCost;
            return View(prescription);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var prescription = await _db.Prescriptions.FindAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            var drugs = await _db.PrescriptionDrugs
                .Include(a => a.Drug)
                .Where(a =>
                    a.PrescriptionId.Equals(prescription.Id) &&
                    a.IsWantToBuy.Equals(true))
                .ToListAsync();

            foreach (var drug in drugs)
            {
                drug.IsBought = true;
                _db.PrescriptionDrugs.Update(drug);
                await _db.SaveChangesAsync();
            }

            prescription.Status = "ارسال نشده";
            prescription.PaymentMethod = "غیر نقدی";

            _db.Prescriptions.Update(prescription);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پرداخت با موفقیت انجام شد";
            return RedirectToAction("PrescriptionDetails", "Home", new { id = prescription.Id });
        }

        #endregion

        #region EditProfile

        public async Task<IActionResult> EditProfile()
        {
            return View(await _db.Patients
                .FirstOrDefaultAsync(a =>
                    a.Username.Equals(User.Identity.Name)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Models.DomainClasses.Users.Patient patient, IFormFile profilePic)
        {
            var oldPassword = _db.Patients.AsNoTracking().FirstOrDefault(a => a.Id.Equals(patient.Id))?.Password;
            var oldProfilePic = _db.Patients.AsNoTracking().FirstOrDefault(a => a.Id.Equals(patient.Id))?.ProfilePic;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(oldProfilePic))
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(patient.Password))
            {
                if (patient.Password.Count() < 6)
                {
                    ModelState.AddModelError("Password", "کلمه عبور باید حداقل شامل ۶ کاراکتر باشد");
                }
                else
                {
                    patient.Password = _loginService.GetHash(patient.Password);
                }
            }
            else
            {
                patient.Password = oldPassword;
            }

            if (!ModelState.IsValid)
            {
                patient.ProfilePic = oldProfilePic;
                return View(patient);
            }

            if (!(profilePic == null || profilePic.Length == 0))
            {
                if (profilePic.Length > 0 && profilePic.Length < 500000)
                    try
                    {
                        if (!string.IsNullOrEmpty(oldProfilePic) && oldProfilePic != "avatat-11.png")
                        {
                            var oldImage = oldProfilePic;
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/Administrators/assets/images/patients/", oldImage);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            else
                            {
                                _logger.LogError($"The old image path cannot be found. Path = {oldImagePath}");
                                TempData["Error"] = "خطا در حذف عکس";
                                return RedirectToAction("EditProfile");
                            }
                        }

                        patient.ProfilePic = Guid.NewGuid() + Path.GetExtension(profilePic.FileName);
                        string savePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/patients",
                            patient.ProfilePic
                        );
                        await using var stream = new FileStream(savePath, FileMode.Create);
                        await profilePic.CopyToAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Image replacement incomplete. error = {ex.Message}");
                    }
                else
                {
                    TempData["Error"] = "حجم عکس بارگذاری شده برای پروفایل پزشک بیشتر از 500 کیلوبایت می باشد";
                    return RedirectToAction("EditProfile");
                }
            }
            else
            {
                patient.ProfilePic = oldProfilePic;
            }

            patient.IsValidated = true;

            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پروفایل با موفقیت ذخیره شد";
            return View(patient);
        }

        #endregion

        public async Task<IActionResult> Chat(string reserveDate, int doctorId = 0, int patientId = 0)
        {
            if (doctorId == 0 || patientId == 0 || string.IsNullOrEmpty(reserveDate))
            {
                return NotFound();
            }

            if (!DateTime.TryParse(reserveDate, out var reservDateTime))
            {
                return NotFound();
            }

            var doctor = await _db.Doctors.FindAsync(doctorId);
            var patient = await _db.Patients.FindAsync(patientId);

            if (doctor == null || patient == null)
            {
                return NotFound();
            }

            var isQualified = await _db.Visits
                .AnyAsync(a =>
                    a.ReservationDoctorId.Equals(doctorId) &&
                    a.ReservationPatientId.Equals(patientId) &&
                    a.ReservationReserveDate.Equals(reservDateTime) &&
                    a.ChatFlag.Equals(false));

            if (!isQualified)
            {
                TempData["Error"] = "شما نمی توانید بیش از یک بار به ازای هر ویزیت، گفتگوی آنلاین داشته باشید";
                return RedirectToAction("VisitList");
            }

            var viewModel = new DoctorPatientViewModel(doctor, patient);

            ViewBag.DoctorId = doctorId;
            ViewBag.PatientId = patientId;
            ViewBag.ReserveDateTime = reservDateTime;
            return View(viewModel);
        }
    }
}