using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Clinic.WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "صفحه مورد نظر شما یافت نشد";
                    _logger.LogError($"404 Error occured. Path = {statusCodeResult.OriginalPath}" +
                                     $" and Query string = {statusCodeResult.OriginalQueryString}");
                    break;
                case 500:
                    ViewBag.ErrorMessage = "عدم پاسخگویی سرور. تیم فنی در حال بررسی مشکل است";
                    _logger.LogError($"500 Error occured. Path = {statusCodeResult.OriginalPath}" +
                                     $" and Query string = {statusCodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound", statusCode);
        }
    }
}