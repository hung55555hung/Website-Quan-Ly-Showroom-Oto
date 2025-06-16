using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPBL3.DTO;
using WebPBL3.Models;
using System.Text.Json;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICarService _carService;
        public OrderController(IOrderService orderService, ICarService carService)
        {
            _orderService = orderService;
            _carService = carService;   
        }
        public async Task<IActionResult> Index(string status="",string idUser="" ,int page = 1)
        {
            if(!status.IsNullOrEmpty())
            {
                ViewBag.status = status;
                
            }
            if (!idUser.IsNullOrEmpty())
            {
                ViewBag.idUser = idUser;
            }
            List<Order> orders =await _orderService.GetOrders(status, idUser);
            double total = orders.Count;
            var totalPage = (int)Math.Ceiling(total / 10);
            if (page < 1)
                page = 1;
            if (page > totalPage) 
                page = totalPage;
            ViewBag.totalPage = totalPage;
            ViewBag.currentPage = page;
            orders = orders.Skip((page - 1) * 10).Take(10).ToList();
            return View(orders);
        }
        [Authorize(Roles = "Staff")]
        public IActionResult Creat() 
        {
            OrderDTO orderDTO;
            string orderDTOJson = TempData["orderDTO"] as string;
            if (orderDTOJson == null)
            {
                orderDTO = new OrderDTO();
            }
            else
            {
                orderDTO = JsonConvert.DeserializeObject<OrderDTO>(orderDTOJson);
            }
            return View(orderDTO);
        }
        [HttpPost]
        public async Task<IActionResult> ExtractEmail(string existEmail)
        {
            try
            {
                string orderDTOJson = JsonConvert
                    .SerializeObject(await _orderService.getInforByEmail(existEmail));
                TempData["orderDTO"] = orderDTOJson; 
                return RedirectToAction("Creat");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Creat");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] Items item)
        {
            var car = await _carService.GetCarById(item.carID);
            if (car == null)
            {
                return NotFound("Mã xe không tồn tại");
            }
            if (car.Quantity < item.quantity)
            {
                return BadRequest("Số lượng xe không đủ");
            }
            item.carName = car.CarName;
            item.price = car.Price;
            item.color = car.Color;
            Console.WriteLine(Json(item));
            return Json(item);   
        }
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Creat(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(orderDTO);
            }
            await _orderService.CreateOrder(orderDTO);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                DetailOrderDTO detail =await _orderService.GetDetailOrder(id);
                return View(detail);
            }catch(Exception ex)
            {
                return NotFound();
            }
        }
    
        public async Task<IActionResult> EditOrder(string id)
        {
            await _orderService.UpdateOrder(id);
            return RedirectToAction("Index");
        }
        
        
        public async Task<IActionResult> DeleteOrder(string id)
        {
            await _orderService.DeleteOrder(id);
            return RedirectToAction("Index");
        }
    }
}
