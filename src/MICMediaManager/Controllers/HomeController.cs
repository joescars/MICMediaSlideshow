using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MICMediaManager.Models;
using Microsoft.Extensions.Options;

namespace MICMediaManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyOptions _optionsAccessor;
        public HomeController(IOptions<MyOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor.Value;
        }
        public IActionResult Index()
        {
            ViewData["StorageAccountName"] = _optionsAccessor.StorageAccountName;
            ViewData["StorageAccountKey"] = _optionsAccessor.StorageAccountKey;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
