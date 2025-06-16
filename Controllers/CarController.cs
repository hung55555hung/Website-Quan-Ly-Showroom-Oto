using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TextTemplating;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using WebPBL3.Models;
using WebPBL3.DTO;
using Microsoft.AspNetCore.Authorization;
using WebPBL3.Services;
using Microsoft.Identity.Client;
namespace WebPBL3.Controllers
{
    public class CarController : Controller
    {
        private ICarService _carService;
        private IPhotoService _photoService;
        // Số lượng item mỗi trang
        private int limits = 10;
        public CarController(ICarService carService, IPhotoService photoService)
        {
            _carService = carService;
            _photoService = photoService;
        }

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            ViewBag.HideHeader = false;
            ViewBag.SearchTerm = searchTerm;
            try
            {
                var makes = await _carService.GetAllMakes();
                var origins = await _carService.GetOrigins();
                var colors = await _carService.GetColors();
                var fuels = await _carService.GetFuelConsumption();
                var seats = await _carService.GetSeats();
            
            

                // Đưa danh sách vào ViewBag
                ViewBag.Makes = new SelectList(makes, "MakeID", "MakeName");
                ViewBag.Origins = new SelectList(origins);
                ViewBag.Colors = new SelectList(colors);
                ViewBag.Fuels = new SelectList(fuels);
                ViewBag.Seats = new SelectList(seats);
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }
        
        public async Task<ActionResult> Cars(string txtSearch = "",string makeName = "", string origin = "", string color = "", string seat = "",int page = 1, int perPage = 9, string sortBy = "")
        {
            try
            {
                var item = await _carService.FilterCars(txtSearch, makeName, origin, color, seat, page, perPage, sortBy);
                int totalCount = item.Count();
                var cars = item.Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(i => new List<string>
                {
                i.CarID,
                i.Photo,
                i.Price.ToString(),
                i.CarName
                });
                int totalPages = (int)Math.Ceiling((double)totalCount / perPage);
                return Json(new { Data = cars, TotalPages = totalPages });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
		}
        
		public async Task<IActionResult> Detail(string id)
        {
			ViewBag.HideHeader = false;
			if (string.IsNullOrEmpty(id))
			{
				return NotFound("Id is null or empty.");
			}
			Car? car = await _carService.GetCarById(id);

			if (car == null)
			{
				return NotFound("Car is not found.");
			}
            try
            {
                CarDTO CarDTOFromDb = _carService.ConvertToCarDTO(car);

                ViewBag.RelatedCars = await _carService.GetRelatedCars(car);
                return View(CarDTOFromDb);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> CarListTable(int makeid = 0, string searchtxt = "", int page = 1)
        {
            try
            {



                // Kiểm tra và lấy dữ liệu "makes" nếu chưa có trong TempData
                // TemData lưu trữ dữ liệu trong Session, khi Session hết hạn hoặc bị xóa thì makes bay
                if (!TempData.ContainsKey("makes"))
                {
                    IEnumerable<Make> makes = await _carService.GetAllMakes();
                    TempData["makes"] = JsonConvert.SerializeObject(makes);
                    TempData.Keep("makes");
                }

                int total = await _carService.CountCars(makeid, searchtxt);
                // tổng số trang
                var totalPage = (total + limits - 1) / limits;
                // sử dụng khi previous là 1
                if (page < 1) page = 1;
                // sử dụng khi next là totalPage 
                if (page > totalPage && totalPage > 0) page = totalPage;
                IEnumerable<CarDTO> cars = await _carService.GetAllCars(makeid, searchtxt, page);
                // tổng số sản phẩm


                ViewBag.totalRecord = total;
                ViewBag.totalPage = totalPage;
                ViewBag.currentPage = page;
                ViewBag.makeid = makeid;
                ViewBag.searchtxt = searchtxt;


                return View(cars);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // [GET]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Create()
        {
            return View();
        }
        // [POST]
        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Create(CarDTO cardto, IFormFile uploadimage)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {


                cardto.CarID = await _carService.GenerateID();

                if (uploadimage != null && uploadimage.Length > 0)
                {
                    cardto.Photo = await _photoService.AddPhoto("car", uploadimage);
                }
                try
                {

                    await _carService.AddCar(cardto);

                }
                catch (DbUpdateException ex)
                {
                    // 404
                    return BadRequest("Error add car: " + ex.Message);

                }
                return RedirectToAction("CarListTable");

            }
            return View(cardto);

        }
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Id is null or empty");
            }
            Car? car = await _carService.GetCarById(id);
            if (car == null)
            {
                return NotFound("Car is not found");
            }
            try
            {
                CarDTO CarDTOFromDb = _carService.ConvertToCarDTO(car);
                return View(CarDTOFromDb);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

            
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(CarDTO cardto, IFormFile? uploadimage)
        {

            if (ModelState.IsValid)
            {
                
                if (string.IsNullOrEmpty(cardto.CarID))
                {
                    return NotFound("Car is not found");
                }
                if (uploadimage != null && uploadimage.Length > 0)
                {
                    cardto.Photo = await _photoService.EditPhoto("car", uploadimage, cardto.Photo);
                }

                try
                {
                    await _carService.EditCar(cardto);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // 404
                    return BadRequest("Error edit car: " + ex.Message);

                }
                return RedirectToAction("CarListTable");
            }
            return View(cardto);

        }
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Details(string? id)
        {
            try
            {


                if (string.IsNullOrEmpty(id))
                {
                    return NotFound("Id is null");
                }
                Car? car = await _carService.GetCarById(id);

                if (car == null)
                {
                    return NotFound("Car is not found");
                }
                CarDTO cardtoFromDb = _carService.ConvertToCarDTO(car);

                return View(cardtoFromDb);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Delete(string? id)
        {
            try
            {


                if (string.IsNullOrEmpty(id))
                {
                    return NotFound("Id is null");
                }
                Car? car = await _carService.GetCarById(id);

                if (car == null)
                {
                    return NotFound("Car is not found");
                }

                try
                {
                    await _carService.DeleteCar(car);

                }
                catch (DbUpdateConcurrencyException ex)
                {

                    return BadRequest("Error delete car: " + ex.Message);
                }

                return RedirectToAction("CarListTable");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}