using WebPBL3.Models;
using WebPBL3.DTO;
namespace WebPBL3.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsers(string searchtxt, int fieldsearch, int page);
        Task<User> GetUserById(string id);
        Task AddUser(UserDTO userdto);
        Task EditUser(UserDTO userdto);
        Task DeleteUser(string accountId);

        User ConvertToUser(UserDTO UserDTO);
        Task<UserDTO> ConvertToUserDTO(User user);
        Task<UserDTO> ExtractEmail(string email);
        Task<string> GenerateID();
        Task<bool> CheckEmailExits(string email);
        Task<int> CountUsers(string searchtxt, int fieldsearch);

    }
}