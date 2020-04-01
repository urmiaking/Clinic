using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.NewsPage;
using Clinic.Models.DomainClasses.Users;
using Clinic.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            var news = _db.News.OrderByDescending(n => n.ReleaseDate).Take(6).ToList();

            //TODO:Add RoleChecking

            return View(news);
        }

        public async Task<IActionResult> DoctorList(string searchString, string currentFilter, int? page)
        {
            //TODO: Add RoleChecking
            
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
            //TODO: Add RoleChecking

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
    }
}