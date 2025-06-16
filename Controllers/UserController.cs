using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DiaSymReader;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Net;
using System.Numerics;
using WebPBL3.DTO;
using WebPBL3.Models;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    [Authorize(Roles = "Admin, Staff")]

    public class UserController : Controller
    {
        IUserService _userService;
        IPhotoService _photoService;

        private int limits = 10;
        public UserController(IUserService userService, IPhotoService photoService)
        {
            _photoService = photoService;
            _userService = userService;

        }
        public IActionResult Index()
        {
            return View();
        }
        // GET
        public async Task<IActionResult> UserListTable(string searchtxt = "", int fieldsearch = 1, int page = 1)
        {

            int total = await _userService.CountUsers(searchtxt, fieldsearch);
            // tổng số trang
            int totalPage = (total + limits - 1) / limits;

            if (page < 1) page = 1;
            if (page > totalPage && total > 0) page = totalPage;
            IEnumerable<UserDTO> users = await _userService.GetAllUsers(searchtxt, fieldsearch, page);
            // tổng số  người dùng

            ViewBag.totalRecord = total;
            ViewBag.totalPage = totalPage;
            ViewBag.currentPage = page;
            ViewBag.searchtxt = searchtxt;
            ViewBag.fieldsearch = fieldsearch;



            return View(users);
        }

        // GET
        public IActionResult Create()
        {

            return View();
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> Create(UserDTO userdto, IFormFile? uploadimage)
        {
            if (ModelState.IsValid)
            {

                bool checkEmailExist = await _userService.CheckEmailExits(userdto.Email);
                if (checkEmailExist)
                {
                    TempData["Error"] = "Email đã tồn tại";
                    return View(userdto);
                }

                userdto.UserID = await _userService.GenerateID();

                if (uploadimage != null && uploadimage.Length > 0)
                {

                    userdto.Photo = await _photoService.AddPhoto("user", uploadimage);

                }
                try
                {
                    userdto.RoleID = 3;
                    await _userService.AddUser(userdto);

                }
                catch (DbUpdateException ex)
                {
                    return BadRequest("Error add user: " + ex.Message);
                }

                return RedirectToAction("UserListTable");

            }
            return View(userdto);
        }
        // GET
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Id is null");
            }
            User? user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound("User is not found");
            }

            UserDTO userdtoFromDb = await _userService.ConvertToUserDTO(user);
            //Console.WriteLine(user.UserID);
            return View(userdtoFromDb);
        }


        // POST
        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userdto, IFormFile? uploadimage)
        {
            if (ModelState.IsValid)
            {
                //Console.WriteLine(userdto.UserID);
                User? user = await _userService.GetUserById(userdto.UserID);

                if (user == null)
                {
                    return NotFound("User is not found");
                }
                if (uploadimage != null && uploadimage.Length > 0)
                {
                    userdto.Photo = await _photoService.EditPhoto("user", uploadimage, userdto.Photo);
                }
                Console.WriteLine("Account " + userdto.AccountID);
                try
                {
                    await _userService.EditUser(userdto);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest("Error edit user: " + ex.Message);

                }


                return RedirectToAction("UserListTable");
            }
            return View(userdto);
        }
        // GET
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Id is null");
            }
            User? user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound("User is not found");
            }

            UserDTO userFromDb = await _userService.ConvertToUserDTO(user);

            return View(userFromDb);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            //Console.WriteLine(id);
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Id is null");
            }

            User? user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User is not found");
            }

            try
            {
                await _userService.DeleteUser(user.AccountID);

            }
            catch (Exception ex)
            {
                return BadRequest("Error delete user and account: " + ex.Message);

            }
            return RedirectToAction("UserListTable");

        }


    }
}