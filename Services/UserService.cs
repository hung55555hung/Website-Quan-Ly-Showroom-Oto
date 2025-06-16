using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private IAccountService _accountService;
        public UserService(ApplicationDbContext db, IWebHostEnvironment environment, IAccountService accountService)
        {
            _db = db;
            _environment = environment;
            _accountService = accountService;
        }
        public async Task AddUser(UserDTO userdto)
        {

            try
            {
                userdto.AccountID = await _accountService.AddDefaultAccount(userdto.Email, userdto.RoleID);
                User user = ConvertToUser(userdto);
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi thêm người dùng: ", ex);
            }

        }
        public async Task DeleteUser(string accountId)
        {

            try
            {
                Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountID == accountId);
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi xóa người dùng: ", ex);
            }
        }

        public async Task EditUser(UserDTO userdto)
        {

            try
            {
                User user = ConvertToUser(userdto);
                // Kiểm tra xem thực thể có đang được theo dõi trong ngữ cảnh không
                var trackedEntity = _db.Users.Local
                    .FirstOrDefault(u => u.UserID == userdto.UserID);

                if (trackedEntity != null)
                {
                    // Hủy theo dõi thực thể hiện tại
                    _db.Entry(trackedEntity).State = EntityState.Detached;
                }

                _db.Users.Update(user);
                await _db.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi cập nhật người dùng: ", ex);
            }
        }
        public async Task<User> GetUserById(string id)
        {

            try
            {
                User? user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == id);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi truy vấn người dùng: ", ex);
            }


        }

        public User ConvertToUser(UserDTO UserDTO)
        {
            try
            {
                User user = new User
                {
                    UserID = UserDTO.UserID,
                    FullName = UserDTO.FullName,
                    PhoneNumber = UserDTO.PhoneNumber,
                    IdentityCard = UserDTO.IdentityCard,
                    Gender = UserDTO.Gender,
                    Address = UserDTO.Address,
                    BirthDate = UserDTO.BirthDate,
                    Photo = UserDTO.Photo,
                    WardID = UserDTO.WardID > 0 ? UserDTO.WardID : null,
                    AccountID = UserDTO.AccountID,
                };
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi UserDTO thành User: ", ex);
            }



        }

        public async Task<UserDTO> ConvertToUserDTO(User user)
        {
            try
            {
            Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountID == user.AccountID);
            if (account == null)
            {
                throw new InvalidOperationException($"Account với ID {user.AccountID} không được tìm thấy.");
            }
            UserDTO UserDTO = new UserDTO
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = account.Email,
                PhoneNumber = user.PhoneNumber,
                IdentityCard = user.IdentityCard,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                Address = user.Address,
                WardID = user.WardID??0,
                Photo = user.Photo,
                AccountID = user.AccountID,
            };
            
            Ward? ward = await _db.Wards.Where(w => w.WardID == user.WardID).FirstOrDefaultAsync();

                if (ward != null)
                {
                    UserDTO.WardName = ward.WardName;
                    District? district = await _db.Districts.FirstOrDefaultAsync(d => d.DistrictID == ward.DistrictID);
                    if (district != null)
                    {
                        UserDTO.DistrictName = district.DistrictName;
                        UserDTO.DistrictID = district.DistrictID;
                        Province? province = await _db.Provinces.FirstOrDefaultAsync(p => p.ProvinceID == district.ProvinceID);
                        if (province != null)
                        {
                            UserDTO.ProvinceName = province.ProvinceName;
                            UserDTO.ProvinceID = province.ProvinceID;
                        }
                    }
                }

                return UserDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi User thành UserDTO: ", ex);
            }

        }



        public async Task<IEnumerable<UserDTO>> GetAllUsers(string searchtxt, int fieldsearch, int page)
        {
            List<User> users = await _db.Users.OrderBy(u => u.UserID)
                .Where(u => u.Account.RoleID == 3 &&
                    (string.IsNullOrEmpty(searchtxt) ||
                    (fieldsearch == 1 && u.FullName.Contains(searchtxt)) ||
                    (fieldsearch == 2 && u.PhoneNumber.Contains(searchtxt)) ||
                    (fieldsearch == 3 && u.Account.Email.Contains(searchtxt)) ||
                    (fieldsearch == 4 && u.IdentityCard.Contains(searchtxt))))
                .Skip((page - 1) * 10)
                .Take(10).ToListAsync();


            List<UserDTO> userdtos = new List<UserDTO>();
            foreach (var user in users)
            {
                UserDTO userdto = await ConvertToUserDTO(user);
                userdtos.Add(userdto);
            }
            return userdtos;
        }

        public async Task<string> GenerateID()
        {
            int userId = 1;
            User? lastUser = await _db.Users.OrderByDescending(u => u.UserID).FirstOrDefaultAsync();
            if (lastUser != null)
            {
                userId = Convert.ToInt32(lastUser.UserID.Substring(2)) + 1;
            }
            string useridTxt = "KH" + userId.ToString().PadLeft(6, '0');
            return useridTxt;
        }
        public async Task<bool> CheckEmailExits(string email)
        {
            try
            {
                Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
                return (account != null);
            }

            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi truy vấn tài khoản: ", ex);
            }
        }

        public async Task<UserDTO> ExtractEmail(string email)
        {
            User? u = await _db.Users
                .Include(a => a.Account)
                .Include(w => w.Ward.District)
                .FirstOrDefaultAsync(u => u.Account.Email == email);
            if (u == null)
            {
                throw new Exception("Email không tồn tại");
            }
            return new UserDTO
            {
                FullName = u.FullName,
                IdentityCard = u.IdentityCard,
                PhoneNumber = u.PhoneNumber,
                Email = email,
                Address = u.Address,
                WardID = u.WardID ?? 0,
                ProvinceID = u.Ward != null ? u.Ward.District.ProvinceID : 0,
                DistrictID = u.Ward != null ? u.Ward.DistrictID : 0,
            };
        }
        public async Task<int> CountUsers(string searchtxt, int fieldsearch)
        {
            return await _db.Users
                .Where(u => u.Account.RoleID == 3 &&
                    (string.IsNullOrEmpty(searchtxt) ||
                    (fieldsearch == 1 && u.FullName.Contains(searchtxt)) ||
                    (fieldsearch == 2 && u.PhoneNumber.Contains(searchtxt)) ||
                    (fieldsearch == 3 && u.Account.Email.Contains(searchtxt)) ||
                    (fieldsearch == 4 && u.IdentityCard.Contains(searchtxt))))
                .CountAsync();

        }
    }
}