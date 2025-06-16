using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface IAccountService
    {
        public Task<string> GenerateID();
        public Task<string> AddDefaultAccount(string email, int role);
        public Task Login(string username, string password);
        public Task Register(RegisterDTO model);
        public Task ForgotPassword(string email);
        public Task ResetPassword(string email,string password);
        public Task<UserDTO> getInforAccount();
        public Task UpdateInforAccount(UserDTO UserDTO, IFormFile? uploadimage);
        public Task ChangePassword(string password,string newPassword);
        public Task<Staff> GetInforStaff(string idUser);
        public Task<IEnumerable<HistoryOrderDTO>> GetHistoryOrders(string email);

    }
}
