using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebPBL3.DTO;
using WebPBL3.Models;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAccountService _accountService;
        private IWebHostEnvironment _environment;
        public AccountController(ApplicationDbContext db, IWebHostEnvironment environment, IAccountService accountService)
        {
            _db = db;
            _accountService = accountService;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _accountService?.Login(model.Email, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.RetypePassword)
                {
                    TempData["Error"] = "Mật khẩu nhập lại không khớp";
                    return View();
                }
                try
                {
                    await _accountService.Register(model);
                    return RedirectToAction("Login");
                }catch(Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return View();
                }
            }
            return View();
        }
        
        public IActionResult Denied()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _accountService.ForgotPassword(email);
                return RedirectToAction("setupNewPassword");
            }catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }
        public IActionResult setupNewPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> setupNewPassword(string otp, string password,
            string retypepassword)
        {
            if (password != retypepassword)
            {
                TempData["Error"] = "Mật khẩu nhập lại không khớp";
                return View();
            }
            var sessionValue = HttpContext.Session.GetString("OTP");
            if (sessionValue != otp &&otp != null)
            {
                TempData["Error"] = "Mã OTP không hợp lệ";
                return View();
            }
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                TempData["Error"] = "Xảy ra lỗi khi truyền dữ liệu. Vui lòng thao tác lại";
                return View();
            }
            try
            {
                await _accountService.ResetPassword(email, password);
                return RedirectToAction("Login");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }
        [Authorize]
        public async Task<IActionResult> InforAccount()
        {

            try
            {
                UserDTO user = await _accountService.getInforAccount();
                if (user.RoleID == 2)
                {
                    var staff = await _accountService.GetInforStaff(user.UserID);
                    ViewBag.Staff = staff;
                }
                if (_db.Orders.Any(o => o.UserID == user.UserID))
                {
                    ViewBag.Unchange = true;
                }
                else
                {
                    ViewBag.Unchange = false;
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InforAccount(UserDTO UserDTO, IFormFile? uploadimage)
        {
            try
            {
                await _accountService.UpdateInforAccount(UserDTO, uploadimage);
                return RedirectToAction("Index", "Home");
            }catch(Exception ex)
            {
                return RedirectToAction("Login");
            } 
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string newPassword, string retypePassword)
        {
            if (newPassword != retypePassword)
            {
                TempData["Error"] = "Mật khẩu nhập lại không khớp";
                return View();
            }
            try
            {
                await _accountService.ChangePassword(password, newPassword);
                return RedirectToAction("Index", "Home");
            }catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
           
        }
        [Authorize(Policy = "User")]
        // [GET]
        public async Task<IActionResult> HistoryOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("Người dùng chưa đăng nhập");
            }
            string? email = User.Identity.Name;

            if (email == null)
            {
                return NotFound("Account is not found");

            }
            IEnumerable<HistoryOrderDTO> historyOrders = await _accountService.GetHistoryOrders(email);
            //Console.WriteLine(historyOrders.Count);
            return View(historyOrders);
        }
    }
}

