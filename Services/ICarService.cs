using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface ICarService
    {
        Task<IEnumerable<CarDTO>> GetAllCars(int makeid, string searchtxt, int page);
        Task<Car> GetCarById(string id);
        Task AddCar(CarDTO cardto);
        Task EditCar (CarDTO cardto);
        Task DeleteCar(Car car);
        Task<IEnumerable<Make>> GetAllMakes();
        CarDTO ConvertToCarDTO(Car car);
        Car ConvertToCar(CarDTO cardto);

        Task<string> GenerateID();

        Task<int> CountCars(int makeid, string searchtxt);

        Task<IEnumerable<string>> GetOrigins();
        Task<IEnumerable<string>> GetColors();
        Task<IEnumerable<string>> GetFuelConsumption();
        Task<IEnumerable<int>> GetSeats();

        Task<IEnumerable<Car>> FilterCars(string txtSearch, string makeName, string origin, string color, string seat, int page, int perPage, string sortBy);

        Task<IEnumerable<Car>> GetRelatedCars(Car car);

    }
}