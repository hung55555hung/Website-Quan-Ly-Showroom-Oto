using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPBL3.Models;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        public async Task<JsonResult> GetProvince()
        {
            return new JsonResult(await _addressService.GetProvinces());
        }

        public async Task<JsonResult> GetDistrict(int id)
        {
            return new JsonResult(await _addressService.GetDistricts(id));
        }

        public async Task<JsonResult> GetWard(int id)
        {
            return new JsonResult(await _addressService.GetWards(id));
        }
    }
}
