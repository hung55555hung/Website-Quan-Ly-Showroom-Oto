using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebPBL3.DTO;
using WebPBL3.Models;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly IStatisticService _statisticService;

        public HomeController(ILogger<HomeController> logger,IStatisticService statisticService)
        {
            _logger = logger;
            _statisticService = statisticService;
            
        }
        

        public async Task<IActionResult> Index()
        {
            ViewBag.HideHeader = false;
            
            ViewBag.news = await _statisticService.GetBestNews();
            ViewBag.cars = await _statisticService.GetBestCars(); 
            ViewBag.feedBacks = await _statisticService.GetBestFeedBacks();
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                string photo = await _statisticService.GetPhotoByEmail(email);
                TempData["avatar"] = photo;
            }
            return View();

        }

        public IActionResult About()
        {
            ViewBag.HideHeader = false;
            return View(); 
        }
        public IActionResult Contact()
        {
            ViewBag.HideHeader = false;
            //List<Province> provinces = new List<Province>();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
