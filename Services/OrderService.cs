using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public OrderService(ApplicationDbContext _db, IHttpContextAccessor _http
            ,IUserService userService){ 
            _context = _db;
            _httpContextAccessor = _http;
            _userService = userService;
        }
        public async Task CreateOrder(OrderDTO orderDTO)
        {
            User? u = _context.Users
                .Include(a => a.Account)
                .FirstOrDefault(u => u.Account.Email == orderDTO.Email);
            Staff? staff = _context.Staffs
                .Include(u => u.User.Account)
                .FirstOrDefault(u => u.User.Account.Email == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
            if (u == null)
            {
                var account_id = 1;
                var lastAccount = _context.Accounts.OrderByDescending(a => a.AccountID).FirstOrDefault();
                if (lastAccount != null)
                {
                    account_id = Convert.ToInt32(lastAccount.AccountID) + 1;
                }
                var accountID = account_id.ToString().PadLeft(8, '0');
                Account a = new Account
                {
                    Email = orderDTO.Email,
                    AccountID = accountID,
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Status = true,
                    RoleID = 3,
                };
                _context.Accounts.Add(a);
                var user_id = 1;
                var lastUser = _context.Users.OrderByDescending(u => u.UserID).FirstOrDefault();
                if (lastUser != null)
                {
                    user_id = Convert.ToInt32(lastUser.UserID.Substring(2)) + 1;
                }
                var userID = "KH" + user_id.ToString().PadLeft(6, '0');
                u = new User
                {
                    UserID = userID,
                    FullName = orderDTO.FullName,
                    Address = orderDTO.Address,
                    IdentityCard = orderDTO.IdentityCard,
                    PhoneNumber = orderDTO.PhoneNumber,
                    AccountID = a.AccountID,
                    WardID = orderDTO.WardID
                };
                _context.Users.Add(u);
            }
            else
            {
                u.FullName = orderDTO.FullName;
                u.Address = orderDTO.Address;
                u.IdentityCard = orderDTO.IdentityCard;
                u.PhoneNumber = orderDTO.PhoneNumber;
                u.WardID = orderDTO.WardID;
                _context.Users.Update(u);
            }
            var order_id = 1;
            var lastOrder = _context.Orders.OrderByDescending(o => o.OrderID).FirstOrDefault();
            if (lastOrder != null)
            {
                order_id = Convert.ToInt32(lastOrder.OrderID.Substring(2)) + 1;
            }
            var orderID = "DH" + order_id.ToString().PadLeft(6, '0');
            Order order = new Order
            {
                OrderID = orderID,
                Date = orderDTO.Date,
                Totalprice = orderDTO.Totalprice,
                Status = orderDTO.Status,
                Flag = true,
                UserID = u.UserID,
                StaffID = staff.StaffID
            };
            _context.Orders.Add(order);
            foreach (var item in orderDTO.items)
            {
                DetailOrder detailOrder = new DetailOrder
                {
                    DetailOrderID = Guid.NewGuid().ToString().Substring(0, 10),
                    Quantity = item.quantity,
                    Price = item.price,
                    OrderID = order.OrderID,
                    CarID = item.carID
                };
                var c = _context.Cars.Find(item.carID);
                c.Quantity -= item.quantity;
                _context.DetailOrders.Add(detailOrder);
            }
            await _context.SaveChangesAsync();
        }


    

        public async Task<DetailOrderDTO> GetDetailOrder(string id)
        {
                Order? order = _context.Orders
                .Include(u => u.User.Account)
                .Include(st => st.Staff.User.Account)
                .FirstOrDefault(o => o.OrderID == id);
            if(order == null)
            {
                throw new Exception("Not found");
            }
                Ward w = _context.Wards.Include(w => w.District.Province).FirstOrDefault(w => w.WardID == order.User.WardID);
                var details = _context.DetailOrders
                         .Include(c => c.Car)
                         .Where(o => o.OrderID == id)
                         .ToList();
                DetailOrderDTO detailOrderDTO = new DetailOrderDTO
                {
                    OrderId = order.OrderID,
                    CustomerName = order.User.FullName,
                    Address = order.User.Address + ", " + w.WardName + ", " + w.District.DistrictName + ", " + w.District.Province.ProvinceName,
                    EmailCustomer = order.User.Account.Email,
                    Phone = order.User.PhoneNumber,
                    StaffId = order.Staff.StaffID,
                    StaffName = order.Staff.User.FullName,
                    EmailStaff = order.Staff.User.Account.Email,
                    PurchaseDate = order.Date,
                    Status = order.Status,
                    ToTalPrice = order.Totalprice
                };
                foreach (var item in details)
                {
                    detailOrderDTO.items.Add(new Items
                    {
                        carID = item.CarID,
                        carName = item.Car.CarName,
                        color = item.Car.Color,
                        price = item.Price,
                        quantity = item.Quantity,
                    });
                }
                return detailOrderDTO;
        }

        public async Task<List<Order>> GetOrders(string status, string idUser)
        {
            List<Order> orders =await _context.Orders.Where(o => (status.IsNullOrEmpty() || o.Status.Contains(status))
                                && (idUser.IsNullOrEmpty() || o.UserID.Contains(idUser))).ToListAsync();
            return orders;
        }

        public async Task UpdateOrder(string id)
        {
            Order o = _context.Orders.FirstOrDefault(o => o.OrderID == id);
            if (o != null)
            {
                o.Status = "Đã thanh toán";
                _context.Update(o);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OrderDTO> getInforByEmail(string email)
        {
            try
            {
               UserDTO user=await _userService.ExtractEmail(email);
                return new OrderDTO {
                    FullName = user.FullName,
                    IdentityCard = user.IdentityCard,
                    PhoneNumber = user.PhoneNumber,
                    Email = email,
                    Address = user.Address,
                    WardID = user.WardID ?? 0,
                    ProvinceID = user.ProvinceID,
                    DistrictID = user.DistrictID,
                };
            }catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteOrder(string id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == id);
            if (order != null && order.Status == "Chưa thanh toán")
            {
                var details = _context.DetailOrders.Where(d => d.OrderID == id).ToList();
                _context.DetailOrders.RemoveRange(details);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
