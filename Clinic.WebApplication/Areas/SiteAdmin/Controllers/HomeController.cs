using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.NewsPage;
using Clinic.Services.LoginService;
using Clinic.Utilities.Pagination;
using Clinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Areas.SiteAdmin.Controllers
{
    [Area("SiteAdmin")]
    [Authorize(Roles = "SiteAdmin")]
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
            var news = await _db.News
                .Include(a => a.SiteAdmin)
                .OrderByDescending(a => a.ReleaseDate)
                .Take(6)
                .ToListAsync();

            ViewBag.NewsCount = news.Count;

            int newsVisitCountAll = 0;
            int newsVisitCountThisMonth = 0;
            foreach (var newsItem in news)
            {
                newsVisitCountAll += newsItem.VisitCount;
                if ((newsItem.ReleaseDate - DateTime.Now).TotalDays < 30)
                {
                    newsVisitCountThisMonth += newsItem.VisitCount;
                }
            }
            ViewBag.CountAll = newsVisitCountAll;
            ViewBag.CountThisMonth = newsVisitCountThisMonth;

            return View(news);
        }

        public IActionResult AddNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNews(NewsImageTagsViewModel newsViewModel)
        {
            if (string.IsNullOrEmpty(newsViewModel.Tags))
            {
                ModelState.AddModelError("Tags","لطفا حداقل یک برچسب وارد کنید");
            }
            if (!ModelState.IsValid)
            {
                return View(newsViewModel);
            }

            newsViewModel.News.ReleaseDate = DateTime.Now;
            newsViewModel.News.SiteAdmin = await _db.SiteAdmins.
                FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            if (newsViewModel.File.Length > 0 && newsViewModel.File.Length < 500000)
            {
                try
                {
                    newsViewModel.News.ImageName = Guid.NewGuid() + Path.GetExtension(newsViewModel.File.FileName);
                    string savePath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/news",
                        newsViewModel.News.ImageName
                    );
                    await using var stream = new FileStream(savePath, FileMode.Create);
                    await newsViewModel.File.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    //TODO: Log error
                    Console.WriteLine(ex.Data);
                }
            }
            else
            {
                TempData["Error"] = "حجم عکس بارگذاری شده برای خبر بیشتر از 500 کیلوبایت می باشد";
                return RedirectToAction("DoctorsList");
            }

            await _db.News.AddAsync(newsViewModel.News);
            await _db.SaveChangesAsync();

            string[] tags = newsViewModel.Tags.Split(',');
            foreach (var tagValue in tags)
            {
                NewsTag newsTag = null;

                var availableTag = await _db.Tags.FirstOrDefaultAsync(a => a.TagValue.Equals(tagValue));
                if (availableTag != null)
                {
                    newsTag = new NewsTag()
                    {
                        News = newsViewModel.News,
                        Tag = availableTag
                    };
                }
                else
                {
                    var tag = new Tag()
                    {
                        TagValue = tagValue
                    };
                    await _db.Tags.AddAsync(tag);
                    await _db.SaveChangesAsync();

                    newsTag = new NewsTag()
                    {
                        Tag = tag,
                        News = newsViewModel.News
                    };
                }
                await _db.NewsTags.AddAsync(newsTag);
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "خبر با موفقیت اضافه شد";
            return RedirectToAction("News");
        }

        public async Task<IActionResult> News(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var news = _db.NewsTags
                .Include(a => a.News)
                    .ThenInclude(a => a.SiteAdmin)
                .Include(a => a.Tag)
                .OrderByDescending(a => a.News.ReleaseDate);

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            if (string.IsNullOrEmpty(searchString))
            {
                return View(await PaginatedList<NewsTag>.CreateAsync(news.AsNoTracking(), pageNumber, pageSize));
            }

            var searchNewsResults = _db.NewsTags
                .Include(a => a.News)
                    .ThenInclude(a => a.SiteAdmin)
                .Include(a => a.Tag)
                .OrderByDescending(a => a.News.ReleaseDate)
                .Where(a =>
                    a.News.Title.Contains(searchString) ||
                    a.News.Description.Contains(searchString) ||
                    a.News.ShortDescription.Contains(searchString));

            var searchTagResults = _db.NewsTags
                .Include(a => a.News)
                    .ThenInclude(a => a.SiteAdmin)
                .Include(a => a.Tag)
                .OrderByDescending(a => a.News.ReleaseDate)
                .Where(a => a.Tag.TagValue.Equals(searchString));


            var searchResult = searchNewsResults.Union(searchTagResults);

            return View(await PaginatedList<NewsTag>.CreateAsync(searchResult.AsNoTracking(), pageNumber, pageSize));
        }
    }
}