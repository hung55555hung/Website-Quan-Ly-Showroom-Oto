using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using WebPBL3.DTO;
using WebPBL3.Models;
namespace WebPBL3.Services
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment environment;
        IUserService _userService;
        public StaffService(ApplicationDbContext db, IWebHostEnvironment environment, IUserService userService)
        {
            _db = db;
            this.environment = environment;
            _userService = userService; 
        }
        public async Task<List<StaffDTO>> GetAllStaffs()
        {
            var staff = await _db.Staffs
                .Include(s => s.User)
                .ThenInclude(u => u.Ward)
                .ThenInclude(w => w.District)
                .ThenInclude(d => d.Province)
                .Include(s => s.User.Account)
                .OrderBy(s => s.StaffID)
                .Select(s => new StaffDTO
                {
                    StaffID = s.StaffID,
                    FullName = s.User.FullName,
                    Email = s.User.Account.Email,
                    PhoneNumber = s.User.PhoneNumber,
                    IdentityCard = s.User.IdentityCard,
                    Gender = s.User.Gender,
                    BirthDate = s.User.BirthDate,
                    Address = s.User.Address,
                    Position = s.Position,
                    Salary = s.Salary,
                    ProvinceName = s.User.Ward.District.Province.ProvinceName,
                    DistrictName = s.User.Ward.District.DistrictName,
                    WardName = s.User.Ward.WardName

                }).ToListAsync();
            return staff;
        }
        public async Task<IEnumerable<StaffDTO>> GetStaffsBySearch(string searchTerm, string searchField)
        {
            IQueryable<StaffDTO> staffQuery = _db.Staffs
            .Include(s => s.User)
            .ThenInclude(u => u.Ward)
            .ThenInclude(w => w.District)
            .ThenInclude(d => d.Province)
            .Include(s => s.User.Account)
            .Select(s => new StaffDTO
            {
                StaffID = s.StaffID,
                FullName = s.User.FullName,
                Email = s.User.Account.Email,
                PhoneNumber = s.User.PhoneNumber,
                IdentityCard = s.User.IdentityCard,
                Gender = s.User.Gender,
                BirthDate = s.User.BirthDate,
                Address = s.User.Address,
                Position = s.Position,
                Salary = s.Salary,
                ProvinceName = s.User.Ward.District.Province.ProvinceName,
                DistrictName = s.User.Ward.District.DistrictName,
                WardName = s.User.Ward.WardName
            });

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "FullName":
                        staffQuery = staffQuery.Where(s => s.FullName != null && s.FullName.Contains(searchTerm));
                        break;
                    case "IdentityCard":
                        staffQuery = staffQuery.Where(s => s.IdentityCard != null && s.IdentityCard.Contains(searchTerm));
                        break;
                    case "PhoneNumber":
                        staffQuery = staffQuery.Where(s => s.PhoneNumber != null && s.PhoneNumber.Contains(searchTerm));
                        break;
                    case "Address":
                        staffQuery = staffQuery.Where(s => s.Address != null && s.Address.Contains(searchTerm));
                        break;
                }
            }
            return staffQuery;
        }
        public async Task<FullStaffInfoDTO> GetStaffById(string id)
        {
            try
            {
                Staff? staff = await _db.Staffs.FirstOrDefaultAsync(u => u.StaffID == id);
                if (staff == null) return null;

                User? user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == staff.UserID);
                if (user == null) return null;

                Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountID == user.AccountID);
                if (account == null) return null;

                Ward? ward = await _db.Wards.FirstOrDefaultAsync(w => w.WardID == user.WardID);
                District? district = null;
                Province? province = null;

                if (ward != null)
                {
                    district = await _db.Districts.FirstOrDefaultAsync(d => d.DistrictID == ward.DistrictID);
                    province = await _db.Provinces.FirstOrDefaultAsync(p => p.ProvinceID == district.ProvinceID);
                }

                string? districtName = district?.DistrictName;
                string? provinceName = province?.ProvinceName;
                string? wardName = ward?.WardName;
                int? districtID = district?.DistrictID;
                int? provinceID = province?.ProvinceID;
                int? wardID = ward?.WardID;

                return new FullStaffInfoDTO
                {
                    Staff = staff,
                    User = user,
                    Account = account,
                    WardName = wardName,
                    DistrictName = districtName,
                    ProvinceName = provinceName,
                    WardID = wardID,
                    DistrictID = districtID,
                    ProvinceID = provinceID
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi truy vấn người dùng: ", ex);
            }
        }
        public async Task AddStaff(StaffDTO staffDTO)
        {
            try
            {
                Staff staff = ConvertToStaff(staffDTO);
                _db.Staffs.Add(staff);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi thêm người dùng: ", ex);
            }
        }
        public async Task EditStaff(Staff staff)
        {
            try
            {
                _db.Staffs.Update(staff);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi cập nhật người dùng: ", ex);
            }
        }
        public async Task DeleteStaff(Staff staff)
        {
            try
            {
                _db.Staffs.Remove(staff);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi xóa người dùng: ", ex);
            }
        }

        public Staff ConvertToStaff(StaffDTO StaffDTO)
        {
            try
            {
                Staff staff = new Staff
                {
                    StaffID = StaffDTO.StaffID,
                    Position = StaffDTO.Position,
                    Salary = StaffDTO.Salary,
                    UserID = StaffDTO.UserID
                };
                return staff;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi StaffDTO thành Staff: ", ex);
            }
        }
        public User ConvertToUser(StaffDTO StaffDTO, string? photo)
        {
            try
            {
                User user = new User
                {
                    UserID = StaffDTO.UserID,
                    FullName = StaffDTO.FullName,
                    PhoneNumber = StaffDTO.PhoneNumber,
                    IdentityCard = StaffDTO.IdentityCard,
                    Gender = StaffDTO.Gender,
                    Address = StaffDTO.Address,
                    BirthDate = StaffDTO.BirthDate,
                    Photo = photo,
                    WardID = Convert.ToInt32(StaffDTO.WardName),
                    AccountID = StaffDTO.AccountID,
                };
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi StaffDTO thành Staff: ", ex);
            }
        }
        public Account ConvertToAccount(StaffDTO StaffDTO, int roleId, string hashedPassword)
        {
            try
            {
                Account account = new Account
                {
                    AccountID = StaffDTO.AccountID,
                    Email = StaffDTO.Email,
                    Status = false,
                    Password = hashedPassword,
                    RoleID = roleId,
                };
                return account;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi StaffDTO thành Staff: ", ex);
            }
        }
        public async Task<string> GenerateID()
        {
            int staffId = 1;
            Staff? lastUser = await _db.Staffs.OrderByDescending(u => u.StaffID).FirstOrDefaultAsync();
            if (lastUser != null)
            {
                staffId = Convert.ToInt32(lastUser.StaffID.Substring(2)) + 1;
            }
            string staffIdTxt = "NV" + staffId.ToString().PadLeft(6, '0');
            return staffIdTxt;
        }
        public async Task<bool> CheckIdentityCardExits(string? identityCard)
        {
            try
            {
                User? user = await _db.Users.FirstOrDefaultAsync(a => a.IdentityCard == identityCard);
                return (user != null);
            }

            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi truy vấn tài khoản: ", ex);
            }
        }
        public Task<StaffDTO> ConvertToStaffDTO(Staff staff, User user, Account account)
        {
            try
            {
                Ward? ward = _db.Wards.FirstOrDefault(w => w.WardID == user.WardID);
                District? district = null;
                Province? province = null;

                if (ward != null)
                {
                    district = _db.Districts.FirstOrDefault(d => d.DistrictID == ward.DistrictID);
                    province = _db.Provinces.FirstOrDefault(p => p.ProvinceID == district.ProvinceID);
                }
                string? districtName = district?.DistrictName;
                string? provinceName = province?.ProvinceName;
                string? wardName = ward?.WardName;
                StaffDTO staffDto = new StaffDTO
                {
                    StaffID = staff.StaffID,
                    FullName = user.FullName,
                    Email = account.Email,
                    PhoneNumber = user.PhoneNumber,
                    IdentityCard = user.IdentityCard,
                    Gender = user.Gender,
                    BirthDate = user.BirthDate,
                    Address = user.Address,
                    Position = staff.Position,
                    Salary = staff.Salary,
                    ProvinceName = districtName,
                    DistrictName = provinceName,
                    WardName = wardName,
                };
                return Task.FromResult(staffDto); ;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi User thành UserDTO: ", ex);
            }
        }
        public async Task<string?> SaveStaffPhoto(IFormFile? photo)
        {
            if (photo != null && photo.Length > 0)
            {
                string newFilename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(photo.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "upload", "staff", newFilename);
                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                return newFilename;
            }
            return null;
        }

    }
}
