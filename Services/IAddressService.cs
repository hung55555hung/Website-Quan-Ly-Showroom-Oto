using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface IAddressService
    {
        public Task<List<Province>> GetProvinces();
        public Task<Object> GetDistricts(int idProvince);
        public Task<Object> GetWards(int idDistrict);
    }
}
