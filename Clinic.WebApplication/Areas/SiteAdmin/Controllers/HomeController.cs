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
using Microsoft.Extensions.Logging;

namespace Clinic.WebApplication.Areas.SiteAdmin.Controllers
{
    [Area("SiteAdmin")]
    [Authorize(Roles = "SiteAdmin")]
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

        #region News

        public async Task<IActionResult> Index()
        {
            var news = await _db.News
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
            newsViewModel.News.ReleaseDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return View(newsViewModel);
            }

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
                    _logger.LogError($"Image upload incomplete. error = {ex.Message}");
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

            var news = _db.News.OrderByDescending(a => a.ReleaseDate);

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            if (string.IsNullOrEmpty(searchString))
            {
                return View(await PaginatedList<News>.CreateAsync(news.AsNoTracking(), pageNumber, pageSize));
            }

            var searchNewsResults = _db.News
                .OrderByDescending(a => a.ReleaseDate)
                .Where(a =>
                    a.Title.Contains(searchString) ||
                    a.Description.Contains(searchString) ||
                    a.ShortDescription.Contains(searchString) ||
                    a.Tags.Contains(searchString));

            return View(await PaginatedList<News>.CreateAsync(searchNewsResults.AsNoTracking(), pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var newsItem = await _db.News
                .Include(a => a.Comments)
                .ThenInclude(a => a.Replies)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (newsItem == null)
            {
                return StatusCode(404);
            }

            foreach (var comment in newsItem.Comments)
            {
                foreach (var reply in comment.Replies.ToList())
                {
                    _db.Replies.Remove(reply);
                    await _db.SaveChangesAsync();
                }

                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
            }

            _db.News.Remove(newsItem);
            await _db.SaveChangesAsync();

            var oldImage = newsItem.ImageName;
            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/Administrators/assets/images/news/", oldImage);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            else
            {
                _logger.LogError($"The image path cannot be found. Path = {oldImagePath}");
                return StatusCode(402);
            }

            return StatusCode(200);
        }

        public async Task<IActionResult> EditNews(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var news = await _db.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            var newsViewModel = new NewsImageTagsEditViewModel()
            {
                News = news
            };

            return View(newsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(NewsImageTagsEditViewModel newsViewModel, string tagList)
        {
            if (string.IsNullOrEmpty(tagList))
            {
                TempData["Error"] = "خبر حداقل باید یک برچسب داشته باشد";
                return RedirectToAction("EditNews", new { id = newsViewModel.News.Id });
            }

            newsViewModel.News.Tags = tagList;

            if (!ModelState.IsValid)
                return View(newsViewModel);

            if (newsViewModel.File != null && newsViewModel.File.Length != 0)
            {
                var oldImage = newsViewModel.News.ImageName;
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/Administrators/assets/images/news/", oldImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                else
                {
                    TempData["Error"] = "خطا در حذف عکس قبلی";
                    return RedirectToAction("News");
                }

                newsViewModel.News.ImageName = Guid.NewGuid() + Path.GetExtension(newsViewModel.File.FileName);
                string savePath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/Administrators/assets/images/news",
                    newsViewModel.News.ImageName
                );
                await using var stream = new FileStream(savePath, FileMode.Create);
                await newsViewModel.File.CopyToAsync(stream);
            }

            _db.News.Update(newsViewModel.News);
            await _db.SaveChangesAsync();

            TempData["Success"] = "خبر با موفقیت ویرایش شد";
            return RedirectToAction("News");
        }

        public async Task<IActionResult> NewsDetails(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var newsItem = await _db.News
                .Include(a => a.Comments)
                    .ThenInclude(a => a.User)
                .Include(a => a.Comments)
                    .ThenInclude(a => a.Replies)
                        .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            return View(newsItem);
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

            var siteAdmin = await _db.Users.FindAsync(id);
            if (siteAdmin == null)
            {
                return NotFound();
            }
            siteAdmin.Email = newEmail;
            _db.Users.Update(siteAdmin);
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

            var siteAdmin = await _db.Users.FindAsync(id);

            if (siteAdmin == null)
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
            siteAdmin.Password = _loginService.GetHash(newPass);

            _db.Users.Update(siteAdmin);
            await _db.SaveChangesAsync();

            TempData["SuccessPassword"] = "رمز عبور شما با موفقیت تغییر یافت!";
            return RedirectToAction("EditProfile");
        }
        #endregion

        #region CommentReply

        [HttpPost]
        public async Task<IActionResult> AddComment(string commentContent, int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            if (string.IsNullOrEmpty(commentContent))
            {
                return StatusCode(401);
            }

            var news = await _db.News.FindAsync(id);
            var siteAdmin = await _db.Users.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));

            if (news == null || siteAdmin == null)
            {
                return StatusCode(404);
            }

            var newComment = new Comment()
            {
                Content = commentContent,
                News = news,
                User = siteAdmin,
                DateTime = DateTime.Now,
                IsConfirmed = true
            };

            await _db.Comments.AddAsync(newComment);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(string content, int commentId = 0)
        {
            if (commentId == 0)
            {
                return NotFound();
            }

            var comment = await _db.Comments
                .Include(a => a.News)
                .FirstOrDefaultAsync(a => a.Id.Equals(commentId));

            if (comment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(content))
            {
                TempData["Error"] = "لطفا متن نظر خود را وارد کنید";
                return RedirectToAction("NewsDetails", new { id = comment.News.Id });
            }

            var siteAdmin = await _db.Users.FirstOrDefaultAsync(a => a.Username.Equals(User.Identity.Name));
            var reply = new Reply()
            {
                Comment = comment,
                Content = content,
                User = siteAdmin,
                DateTime = DateTime.Now,
                IsConfirmed = true
            };

            await _db.Replies.AddAsync(reply);
            await _db.SaveChangesAsync();

            TempData["Success"] = "نظر شما با موفقیت ثبت شد";
            return RedirectToAction("NewsDetails", new { id = comment.News.Id });
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var comment = await _db.Comments
                .Include(a => a.Replies)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (comment.Replies.Any())
            {
                _db.RemoveRange(comment.Replies);
                await _db.SaveChangesAsync();
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteReply(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var reply = await _db.Replies
                    .Include(a => a.Comment)
                    .ThenInclude(a => a.News)
                    .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (reply == null)
            {
                return StatusCode(404);
            }

            _db.Replies.Remove(reply);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmComment(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var comment = await _db.Comments.FindAsync(id);

            if (comment == null)
            {
                return StatusCode(404);
            }

            comment.IsConfirmed = true;

            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmReply(int id = 0)
        {
            if (id == 0)
            {
                return StatusCode(404);
            }

            var reply = await _db.Replies.FindAsync(id);

            if (reply == null)
            {
                return StatusCode(404);
            }

            reply.IsConfirmed = true;

            _db.Replies.Update(reply);
            await _db.SaveChangesAsync();

            return StatusCode(200);
        }

        #endregion

        #region Messages

        public async Task<IActionResult> Messages()
        {
            return View(await _db.ManagerMessages.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Message(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var message = await _db.ManagerMessages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            message.Seen = true;
            _db.ManagerMessages.Update(message);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پیام خوانده شد";
            return RedirectToAction("Messages");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var message = await _db.ManagerMessages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            _db.ManagerMessages.Remove(message);
            await _db.SaveChangesAsync();

            TempData["Success"] = "پیام با موفقیت حذف شد";
            return RedirectToAction("Messages");
        }

        #endregion
    }
}