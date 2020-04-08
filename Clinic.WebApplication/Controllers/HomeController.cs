using System;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.NewsPage;
using Clinic.Models.DomainClasses.Users;
using Clinic.Services.CaptchaService;
using Clinic.Services.FeedBackService;
using Clinic.Utilities.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IFeedBackService _feedBackService;

        public HomeController(AppDbContext db, IReCaptchaService reCaptchaService, IFeedBackService feedBackService)
        {
            _db = db;
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

            int pageSize = 6;
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

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            if (string.IsNullOrEmpty(searchString))
            {
                var news = _db.News.OrderByDescending(a => a.ReleaseDate);
                return View(await PaginatedList<News>.CreateAsync(news.AsNoTracking(), pageNumber, pageSize));
            }

            var newsResult = _db.News.Where(a =>
                    a.Tags.Contains(searchString) || a.Description.Contains(searchString) ||
                    a.Title.Contains(searchString) ||
                    a.ShortDescription.Contains(searchString))
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
                .Include(a => a.Comments)
                    .ThenInclude(a => a.User)
                .Include(a => a.Comments)
                    .ThenInclude(a => a.Replies)
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

        public async Task<IActionResult> ProfileDetails(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var doctor = await _db.Doctors
                .Include(a => a.WeekDays)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string content, int id = 0, int newsId = 0)
        {
            if (newsId == 0)
            {
                return RedirectToAction("NewsArchive");
            }

            if (id == 0 || string.IsNullOrEmpty(content))
            {
                return RedirectToAction("News", new { id = newsId });
            }

            var news = await _db.News.FindAsync(newsId);
            if (news == null)
            {
                return RedirectToAction("NewsArchive");
            }

            var user = await _db.Users.FindAsync(id);

            bool isConfirmed = !(user is Patient);

            if (user == null)
            {
                return RedirectToAction("News", new { id = newsId });
            }

            var comment = new Comment()
            {
                Content = content,
                DateTime = DateTime.Now,
                News = news,
                User = user,
                IsConfirmed = isConfirmed
            };

            if (!isConfirmed)
            {
                TempData["Success"] = "نظر شما پس از تایید منتشر خواهد شد";
            }
            else
            {
                TempData["Success"] = "نظر شما ثبت شد";
            }

            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();

            return RedirectToAction("News", new { id = newsId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(string content, int id = 0, int commentId = 0, int newsId = 0)
        {
            if (newsId == 0)
            {
                return RedirectToAction("NewsArchive");
            }

            if (id == 0 || string.IsNullOrEmpty(content) || commentId == 0)
            {
                return RedirectToAction("News", new { id = newsId });
            }

            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return RedirectToAction("News", new { id = newsId });
            }

            var comment = await _db.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return RedirectToAction("News", new { id = newsId });
            }

            bool isConfirmed = !(user is Patient);

            var reply = new Reply()
            {
                Comment = comment,
                Content = content,
                DateTime = DateTime.Now,
                User = user,
                IsConfirmed = isConfirmed
            };

            if (!isConfirmed)
            {
                TempData["Success"] = "نظر شما پس از تایید منتشر خواهد شد";
            }
            else
            {
                TempData["Success"] = "نظر شما ثبت شد";
            }

            await _db.Replies.AddAsync(reply);
            await _db.SaveChangesAsync();

            return RedirectToAction("News", new { id = newsId });
        }
    }
}