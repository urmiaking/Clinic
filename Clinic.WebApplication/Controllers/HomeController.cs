using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Microsoft.AspNetCore.Mvc;

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
    }
}