using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using WebPBL3.DTO;
using WebPBL3.Models;
using Microsoft.AspNetCore.Authentication;
using WebPBL3.DTO;
using System.Net.Mail;
using System.Net;
using System;

namespace WebPBL3.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment _environment;
        public AccountService(ApplicationDbContext db,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }
        public async Task<string> AddDefaultAccount(string email, int role)
        {
            try
            {   
                string accountId = await GenerateID();
                _db.Accounts.Add(new Account
                {
                    AccountID = accountId,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Status = false,
                    RoleID = role,
                });
                await _db.SaveChangesAsync();
                return accountId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi thêm tài khoản mặc định: ", ex);
            }
        }

        public async Task<string> GenerateID()
        {
            int accId = 1;

            Account? lastAccount = await _db.Accounts.OrderByDescending(a => a.AccountID).FirstOrDefaultAsync();
            if (lastAccount != null)
            {
                accId = Convert.ToInt32(lastAccount.AccountID) + 1;
            }
            //Console.WriteLine(accid + user.Email);
            var accidTxt = accId.ToString().PadLeft(8, '0');
            return accidTxt;
        }

   


        public async Task Login(string email, string password)
            {
                var user = _db.Accounts.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
            if(user == null)
                {
                    throw new Exception("Tài khoản hoặc mật khẩu không hợp lệ");
                }
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role,user.Role.RoleName)
                };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }
                else
                {
                    throw new Exception("Tài khoản hoặc mật khẩu không hợp lệ");
                }
            }

        public async Task Register(RegisterDTO model)
            {
                var currentAccount = _db.Accounts.FirstOrDefault(u => u.Email == model.Email);
                if (currentAccount != null)
                {
                    throw new Exception("Email đã tồn tại");
                }
                Role role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                var account_id = 1;
                var lastAccount = _db.Accounts.OrderByDescending(a => a.AccountID).FirstOrDefault();
                if (lastAccount != null)
                {
                    account_id = Convert.ToInt32(lastAccount.AccountID) + 1;
                }
                var accountID = account_id.ToString().PadLeft(8, '0');
                var newAccount = new Account
                {
                    AccountID = accountID,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Status = true,
                    RoleID = role.RoleID,
                };
                _db.Accounts.Add(newAccount);
                var user_id = 1;
                var lastUser = _db.Users.OrderByDescending(u => u.UserID).FirstOrDefault();
                if (lastUser != null)
                {
                    user_id = Convert.ToInt32(lastUser.UserID.Substring(2)) + 1;
                }
                var userID = "KH" + user_id.ToString().PadLeft(6, '0');
                var user = new User
                {
                    UserID = userID,
                    AccountID = newAccount.AccountID
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
        public async Task ForgotPassword(string email)
            {
                var existAccount = _db.Accounts.FirstOrDefault(u => u.Email == email);
                if (existAccount == null)
                {
                    throw new Exception("Email không tồn tại!");
                }
                string otp = GenerateOTP();
                _httpContextAccessor.HttpContext.Session.SetString("OTP", otp);
                _httpContextAccessor.HttpContext.Session.SetString("Email", email);
                string message = "Mã OTP: " + otp;
                existAccount.Password = BCrypt.Net.BCrypt.HashPassword(otp);
                _db.Accounts.Update(existAccount);
                await _db.SaveChangesAsync();
                await SendMailGoogleSmtp(email, "Thiết lập mật khẩu mới", message);
            }
        public static async Task SendMailGoogleSmtp(string _to, string _subject,
                                                                string _body)
            {
                string _mail = "thanhpnwork22@gmail.com";
                string _pw = "imid wxgq ttvd ndaz";
                MailMessage message = new MailMessage(
                    from: _mail,
                    to: _to,
                    subject: _subject,
                    body: _body
                );
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_mail, _pw),
                    EnableSsl = true
                };
                await client.SendMailAsync(message);
            }
        private string GenerateOTP()
            {
                Random random = new Random();
                string result = "";
                for (int i = 0; i < 6; i++)
                {
                    result += random.Next(0, 10);
                }
                return result;
            }

        public async Task ResetPassword(string email, string password)
            {
                var existAccount = _db.Accounts.FirstOrDefault(u => u.Email == email);
                if (existAccount == null)
                {
                    throw new Exception("Email không tồn tại");
                }
                existAccount.Password = BCrypt.Net.BCrypt.HashPassword(password);
                _db.Accounts.Update(existAccount);
                await _db.SaveChangesAsync();
            }

        public async Task UpdateInforAccount(UserDTO UserDTO, IFormFile? uploadimage)
            {
                User user = _db.Users.Find(UserDTO.UserID);
                if (user == null)
                {
                    throw new Exception("Not found");
                }
                user.FullName = UserDTO.FullName;
                user.PhoneNumber = UserDTO.PhoneNumber;
                user.IdentityCard = UserDTO.IdentityCard;
                user.Gender = UserDTO.Gender;
                user.BirthDate = UserDTO.BirthDate;
                if (!_db.Users.Any(o => o.UserID == user.UserID))
                {
                    user.Address = UserDTO.Address;
                    user.WardID = UserDTO.WardID??0;
                }
                if (uploadimage != null && uploadimage.Length > 0)
                {
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(uploadimage.FileName);
                    user.Photo = fileName;
                    string _path;
                    if (UserDTO.RoleID == 2)
                    {
                        _path = Path.Combine(_environment.WebRootPath, "upload\\staff", fileName);
                    } else
                    {
                        _path = Path.Combine(_environment.WebRootPath, "upload\\user", fileName);
                    }
                   
                    Console.WriteLine(_path);
                    using (var fileStream = new FileStream(_path, FileMode.Create))
                    {
                        uploadimage.CopyTo(fileStream);
                    }
                }
                 _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }

        public async Task<UserDTO> getInforAccount()
            {
            string email =_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (email == null)
                {
                    throw new Exception("Not found");
                }
                Account account = _db.Accounts.Include(a => a.User.Ward.District).FirstOrDefault(a => a.Email == email);
                if (account == null)
                {
                    throw new Exception("Not found");
                }
            return new UserDTO {
                    UserID = account.User.UserID,
                    RoleID = account.RoleID,
                    Email = account.Email,
                    FullName = account.User.FullName,
                    PhoneNumber = account.User.PhoneNumber,
                    IdentityCard = account.User.IdentityCard,
                    Gender = account.User.Gender,
                    BirthDate = account.User.BirthDate,
                    Address = account.User.Address,
                    Photo = account.User.Photo,
                    WardID = account.User.Ward != null ? account.User.WardID : 0,
                    ProvinceID = account.User.Ward != null ? account.User.Ward.District.ProvinceID : 0,
                    DistrictID = account.User.Ward != null ? account.User.Ward.DistrictID : 0
                };
            }

        public async Task ChangePassword(string password, string newPassword)
            {
                string email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                Account account = _db.Accounts.FirstOrDefault(a => a.Email == email);
                if (account == null)
                {
                    throw new Exception("Xảy ra lỗi, vui lòng đăng nhập lại");
                }
                if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
                {
                    throw new Exception("Mật khẩu không chính xác");
                }
                account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _db.SaveChangesAsync();
            }

        public async Task<Staff> GetInforStaff(string idUser)
        {
           return _db.Staffs.FirstOrDefault(s => s.UserID == idUser);
        }

        public async Task<IEnumerable<HistoryOrderDTO>> GetHistoryOrders(string email)
        {   
            Account? account = await  _db.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Email == email);
            if (account  == null)
            {
                throw new InvalidOperationException("Lỗi trong khi truy vấn tài khoản");
            }
            User? user = account.User;

            if (user == null)
            {
                throw new InvalidOperationException("Lỗi trong khi truy vấn người dùng");
            }

            List<Order> orders = await _db.Orders.Where(o => o.UserID == user.UserID)
                .Include(o => o.DetailOrders)
                .ThenInclude(d => d.Car).ThenInclude(c => c.Make).OrderByDescending(o => o.Date).ToListAsync();


            List<HistoryOrderDTO> historyOrders = new List<HistoryOrderDTO>();

            foreach (var item in orders)
            {
                HistoryOrderDTO historyOrder = new HistoryOrderDTO
                {
                    Date = item.Date,
                    Totalprice = item.Totalprice,
                    Status = item.Status,
                    OrderID = item.OrderID,
                };
                foreach (var itemDetailOrder in item.DetailOrders)
                {
                    historyOrder.Items.Add(new HistoryOrderItem
                    {

                        CarID = itemDetailOrder.CarID,
                        Photo = itemDetailOrder.Car.Photo,
                        CarName = itemDetailOrder.Car.CarName,
                        MakeName = itemDetailOrder.Car.Make.MakeName,
                        Color = itemDetailOrder.Car.Color,
                        Price = itemDetailOrder.Car.Price,
                        Quantity = itemDetailOrder.Quantity
                    });
                }
                historyOrders.Add(historyOrder);
            }
            return historyOrders;
        }
    }
}
