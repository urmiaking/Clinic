using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.NewsPage;
using Clinic.Models.DomainClasses.Others;
using Clinic.Models.DomainClasses.Users;
using Clinic.Services.CaptchaService;
using Clinic.Services.FeedBackService;
using Clinic.Services.MailService;
using Clinic.Utilities.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Clinic.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMailService _mailService;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IFeedBackService _feedBackService;

        public HomeController(AppDbContext db, IMailService mailService, IReCaptchaService reCaptchaService, IFeedBackService feedBackService)
        {
            this._db = db;
            _mailService = mailService;
            _reCaptchaService = reCaptchaService;
            _feedBackService = feedBackService;
        }
        public IActionResult Index()
        {
            var news = _db.News.OrderByDescending(n => n.ReleaseDate).Take(6).ToList();

            if (User.IsInRole("Patient"))
            {
                ViewBag.UserType = "patient";
            }
            else if (User.IsInRole("Doctor"))
            {
                ViewBag.UserType = "doctor";
            }
            else if (User.IsInRole("SiteAdmin"))
            {
                ViewBag.UserType = "admin";
            }
            else if (User.IsInRole("ClinicManager"))
            {
                ViewBag.UserType = "manager";
            }
            else if (User.IsInRole("Pharmacy"))
            {
                ViewBag.UserType = "pharmacy";
            }

            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string name, string email, string message, IFormCollection form)
        {
            string gRecaptchaResponse = form["g-recaptcha-response"];

            var success = await _reCaptchaService.ValidateRecaptchaAsync(gRecaptchaResponse);
            
            if (!success)
            {
                ViewBag.Message = "مشکلی پیش آمد! لطفا گزینه من ربات نیستم را انتخاب کنید";
                return RedirectToAction("Index");
            }

            var isSucceed = await _feedBackService.SendFeedBackAsync(name, email, message);

            if (isSucceed)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Message = "مشکلی پیش آمد لطفا دوباره امتحان کنید";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoctorList(string searchString, string currentFilter, int? page)
        {
            if (User.IsInRole("Patient"))
            {
                ViewBag.UserType = "patient";
            }
            else if (User.IsInRole("Doctor"))
            {
                ViewBag.UserType = "doctor";
            }
            else if (User.IsInRole("SiteAdmin"))
            {
                ViewBag.UserType = "admin";
            }
            else if (User.IsInRole("ClinicManager"))
            {
                ViewBag.UserType = "manager";
            }
            else if (User.IsInRole("Pharmacy"))
            {
                ViewBag.UserType = "pharmacy";
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var doctors = _db.Doctors;

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            if (string.IsNullOrEmpty(searchString))
            {
                return View(await PaginatedList<Doctor>.CreateAsync(doctors.AsNoTracking(), pageNumber, pageSize));
            }

            var searchResults =
                _db.Doctors.Where(a => a.FullName.Contains(searchString) || a.Specialty.Contains(searchString));

            return View(await PaginatedList<Doctor>.CreateAsync(searchResults.AsNoTracking(), pageNumber, pageSize));
        }

        public IActionResult LoginRegisterPanel()
        {
            return View();
        }
        public async Task<IActionResult> NewsArchive(string searchString, string currentFilter, int? page)
        {
            if (User.IsInRole("Patient"))
            {
                ViewBag.UserType = "patient";
            }
            else if (User.IsInRole("Doctor"))
            {
                ViewBag.UserType = "doctor";
            }
            else if (User.IsInRole("SiteAdmin"))
            {
                ViewBag.UserType = "admin";
            }
            else if (User.IsInRole("ClinicManager"))
            {
                ViewBag.UserType = "manager";
            }
            else if (User.IsInRole("Pharmacy"))
            {
                ViewBag.UserType = "pharmacy";
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            if (string.IsNullOrEmpty(searchString))
            {
                var news = _db.News.OrderByDescending(a => a.ReleaseDate);
                return View(await PaginatedList<News>.CreateAsync(news.AsNoTracking(), pageNumber, pageSize));
            }

            var newsResult = _db.NewsTags.Where(a =>
                    a.Tag.TagValue.Equals(searchString) || a.News.Description.Contains(searchString) ||
                    a.News.Title.Contains(searchString) ||
                    a.News.ShortDescription.Contains(searchString)).Select(n => n.News)
                .Distinct()
                .OrderByDescending(a => a.ReleaseDate);

            return View(await PaginatedList<News>.CreateAsync(newsResult,pageNumber, pageSize));
        }

        public async Task<IActionResult> News(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var news = await _db.News
                .Include(a => a.NewsTags)
                    .ThenInclude(a => a.Tag)
                .Include(a => a.Comments)
                    .ThenInclude(a => a.Replies)
                .Include(a => a.Comments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (news == null)
            {
                return NotFound();
            }
            news.VisitCount += 1;
            _db.News.Update(news);
            await _db.SaveChangesAsync();

            var username = User.Identity.Name;
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Username.Equals(username));
            if (user != null)
            {
                ViewBag.UserId = user.Id;
            }

            return View(news);
        }
    }
}