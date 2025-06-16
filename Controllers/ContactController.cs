using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebPBL3.DTO;
using WebPBL3.Models;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    public class ContactController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        public ContactController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        public IActionResult Index()
        {
            ViewBag.HideHeader = false;
            return View();
        }
        [Authorize(Roles ="User")]
        [HttpPost]
        public async Task<IActionResult> Index(FeedbackDTO feedback)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _feedbackService.CreateFeedback(feedback);
                    return RedirectToAction("Index", "Home");
                }catch(Exception ex)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                return View();
            }  
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ListFeedback(DateTime? date=null,string title="")
        {
            List<Feedback> feedbacks =await _feedbackService.GetFeedbacks(date, title);
            ViewBag.DateMessage = date?.ToString("yyyy-MM-dd"); ;
            ViewBag.TitleMessage = title;
            return View(feedbacks);
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteFeedback(string id)
        {
            await _feedbackService.RemoveFeedback(id);
            return RedirectToAction("ListFeedback");
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<JsonResult> getFeedbackById(string id)
        {
            var feedback= await _feedbackService.GetFeedbackById(id);
            return new JsonResult(feedback);
        }
    }
}
