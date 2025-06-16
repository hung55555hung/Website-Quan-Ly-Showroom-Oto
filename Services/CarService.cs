using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private int limits = 10;
        public CarService(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task AddCar(CarDTO cardto)
        {
            try
            {
                Car car = ConvertToCar(cardto);
                _db.Cars.Add(car);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi thêm xe: ", ex);
            }
        }

        public Car ConvertToCar(CarDTO cardto)
        {
            try
            {
                Car car = new Car
                {
                    CarID = cardto.CarID,
                    CarName = cardto.CarName,
                    Photo = cardto.Photo,

                    Capacity = cardto.Capacity,
                    FuelConsumption = cardto.FuelConsumption,
                    Color = cardto.Color,

                    Description = cardto.Description,
                    Dimension = cardto.Dimension,
                    Engine = cardto.Engine,

                    Origin = cardto.Origin,
                    Price = cardto.Price,
                    Quantity = cardto.Quantity,

                    Seat = cardto.Seat,
                    Topspeed = cardto.Topspeed,
                    Year = cardto.Year,

                    MakeID = cardto.MakeID,

                };
                return car;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi CarDTO thành Car: ", ex);
            }


        }

        public CarDTO ConvertToCarDTO(Car car)
        {
            string makeName = _db.Makes.FirstOrDefault(m => m.MakeID == car.MakeID).MakeName;
            try
            {
                CarDTO cardto = new CarDTO
                {
                    CarID = car.CarID,
                    CarName = car.CarName,
                    Photo = car.Photo,
                    Capacity = car.Capacity,
                    FuelConsumption = car.FuelConsumption,
                    Color = car.Color,
                    Description = car.Description,
                    Dimension = car.Dimension,
                    Engine = car.Engine,
                    Origin = car.Origin,
                    Price = car.Price,
                    Quantity = car.Quantity,
                    Seat = car.Seat,
                    Topspeed = car.Topspeed,
                    Year = car.Year,
                    MakeID = car.MakeID,
                    MakeName = makeName,
                };
                return cardto;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi chuyển đổi Car thành CarDTO: ", ex);
            }
        }

        public async Task DeleteCar(Car car)
        {
            try
            {
                car.Flag = true;
                _db.Cars.Update(car);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi xóa xe: ", ex);
            }
        }

        public async Task EditCar(CarDTO cardto)
        {
            try
            {
                Car car = ConvertToCar(cardto);
                _db.Cars.Update(car);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi cập nhật xe: ", ex);
            }
        }

        public async Task<IEnumerable<CarDTO>> GetAllCars(int makeid, string searchtxt, int page)
        {
            try
            {
                List<CarDTO> cars = await _db.Cars
                .OrderBy(c => c.CarID)

                .Where(c => c.Flag == false && (makeid == 0 || c.MakeID == makeid) && (searchtxt.IsNullOrEmpty() || c.CarName.Contains(searchtxt)))
                .Skip((page - 1) * limits).Take(limits)
                .Include(c => c.Make)
                .Select(c => new CarDTO
                {
                    CarID = c.CarID,
                    CarName = c.CarName,
                    Photo = c.Photo,
                    Capacity = c.Capacity,
                    Price = c.Price,
                    Seat = c.Seat,
                    Quantity = c.Quantity,
                    MakeID = c.MakeID,
                    MakeName = c.Make.MakeName,
                }).ToListAsync();
                return cars;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy danh sách xe: ", ex);
            }


        }

        public async Task<IEnumerable<Make>> GetAllMakes()
        {   
            try
            {
                return await _db.Makes.ToListAsync();
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy danh sách hãng: ", ex);
            }
            
        }
        public async Task<Car> GetCarById(string id)
        {
            try
            {
                Car? car = await _db.Cars.FirstOrDefaultAsync(u => u.CarID == id);
                return car;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi truy vấn xe: ", ex);
            }
        }

        public async Task<string> GenerateID()
        {   
            try
            {
                int carId = 1;
                // lấy xe có id lớn nhất
                var lastCar = await _db.Cars.OrderByDescending(c => c.CarID).FirstOrDefaultAsync();
                if (lastCar != null)
                {
                    carId = Convert.ToInt32(lastCar.CarID.Substring(2)) + 1;
                }
                // chuyển về đúng định dạng OT - 6 chữ số
                var caridTxt = "OT" + carId.ToString().PadLeft(6, '0');
                return caridTxt;
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi tạo ID: ", ex);
            }
            
        }
        public async Task<int> CountCars(int makeid, string searchtxt)
        {   
            try
            {
                return await _db.Cars
                .Where(c => c.Flag == false && (makeid == 0 || c.MakeID == makeid) && (searchtxt.IsNullOrEmpty() || c.CarName.Contains(searchtxt))).CountAsync();
            } catch(Exception ex)
            {
                throw new Exception("Lỗi trong khi đếm xe: ", ex);
            }
            
        }

        public async Task<IEnumerable<string>> GetOrigins()
        {   
            try
            {
                return await _db.Cars.Select(c => c.Origin).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy xuất xứ: ", ex);
            }
        }
        public async Task<IEnumerable<string>> GetColors()
        {
            try
            {
                return await _db.Cars.Select(c => c.Color).Distinct().ToListAsync();
            } catch (Exception ex) {
                throw new Exception("Lỗi trong khi lấy danh sách màu: ", ex);
            }
            
        }
        public async Task<IEnumerable<string>> GetFuelConsumption()
        {   
            try
            {
                return await _db.Cars.Select(c => c.FuelConsumption).Distinct().ToListAsync();
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy nhiên liệu: ", ex);
            }
            
        }
        public async Task<IEnumerable<int>> GetSeats()
        {
            try
            {
                return await _db.Cars.Select(c => c.Seat).Distinct().ToListAsync();
            } catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy số chỗ ngồi: ", ex);
            }
            
        }

        public async Task<IEnumerable<Car>> FilterCars(string txtSearch, string makeName, string origin, string color, string seat, int page, int perPage, string sortBy)
        {   
            try
            {
                var item = await _db.Cars
            .Include(c => c.Make)
            .Where(c => !c.Flag) // Lọc những xe không bị gắn cờ
            .ToListAsync();
                if (!string.IsNullOrEmpty(txtSearch))
                {
                    item = item.Where(car => car.CarName.ToLower().Contains(txtSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(makeName))
                {
                    item = item.Where(c => c.Make.MakeName == makeName).ToList();
                }
                if (!string.IsNullOrEmpty(origin))
                {
                    item = item.Where(c => c.Origin == origin).ToList();
                }
                if (!string.IsNullOrEmpty(color))
                {
                    item = item.Where(c => c.Color == color).ToList();
                }
                if (!string.IsNullOrEmpty(seat) && int.TryParse(seat, out int seatNumber))
                {
                    item = item.Where(c => c.Seat == seatNumber).ToList();
                }
                switch (sortBy)
                {
                    case "Price":
                        item = item.OrderBy(p => p.Price).ToList();
                        break;
                    case "bestSelling":
                        var orders = await _db.Orders
                            .Include(o => o.DetailOrders)
                            .ThenInclude(deo => deo.Car)
                            .Where(o => o.Status == "Đã thanh toán")
                            .ToListAsync();
                        Dictionary<string, int> quantity = new Dictionary<string, int>();
                        foreach (var car in item)
                        {
                            quantity[car.CarID] = 0;
                        }
                        foreach (var order in orders)
                        {
                            foreach (var detailOrder in order.DetailOrders)
                            {
                                if (!quantity.ContainsKey(detailOrder.Car.CarID))
                                {
                                    quantity[detailOrder.Car.CarID] = 0;
                                }
                                quantity[detailOrder.Car.CarID] += detailOrder.Quantity;
                            }
                        }
                        item = item.OrderByDescending(c => quantity[c.CarID]).ToList();
                        break;
                }
                return item;
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lọc: ", ex);
            }
            
            
            
        }
        public async Task<IEnumerable<Car>> GetRelatedCars(Car car)
        {   
            try
            {
                return await _db.Cars
                       .Where(c => c.MakeID == car.MakeID && c.CarID != car.CarID && !c.Flag)
                       .ToListAsync();
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy xe liên quan: ", ex);
            }
           
        }

    }
}