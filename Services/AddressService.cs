using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext db)
        {
            _context = db;
        }
        public async Task<Object> GetDistricts(int idProvince)
        {
            var districts =await _context.Districts.Where(d => d.ProvinceID == idProvince)
                .Select(d => new { id = d.DistrictID, name = d.DistrictName }).ToListAsync();
            return districts;
        }

        public async Task<List<Province>> GetProvinces()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<Object> GetWards(int idDistrict)
        {
            var wards =await _context.Wards.Where(w => w.DistrictID == idDistrict)
                .Select(w => new { id = w.WardID, name = w.WardName }).ToListAsync();
            return wards;
        }
    }
}
