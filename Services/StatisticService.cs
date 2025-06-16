using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WebPBL3.DTO;
using WebPBL3.DTO.Statistic;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ApplicationDbContext _db;
        public StatisticService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<string> GetPhotoByEmail(string email)
        {
            Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account != null) 
            {
                User? user = await _db.Users.FirstOrDefaultAsync(u => u.AccountID == account.AccountID);
                return user.Photo;
            }
            return string.Empty;
        }
        public async Task<int> CountCars()
        {
            return  await _db.Cars.Where(c => c.Flag == false).CountAsync();

        }

        public async Task<int> CountFeedBacks()
        {
            return await _db.Feedbacks.CountAsync();
        }

        public async Task<int> CountStaffs()
        {
            return await _db.Staffs.CountAsync();
        }

        public async Task<int> CountUsers()
        {
            return await _db.Users.Include(u => u.Account).Where(u=>u.Account.RoleID == 3).CountAsync();
        }


        public async Task<MemoryStream> CreateAndSaveExcel(List<StatisticTable> statisticTables)
        {
            using (var package = new ExcelPackage())
                {
                    // Tạo một sheet mới trong file Excel
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Viết tiêu đề cho sheet
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "CarID";
                    worksheet.Cells[1, 3].Value = "MakeName";
                    worksheet.Cells[1, 4].Value = "StaffID";
                    worksheet.Cells[1, 5].Value = "Date";
                    worksheet.Cells[1, 6].Value = "Quantity";
                    worksheet.Cells[1, 7].Value = "Price";
                    worksheet.Cells[1, 8].Value = "Total";

                    // Viết dữ liệu vào sheet
                    for (int i = 0; i < statisticTables.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = statisticTables[i].STT;
                        worksheet.Cells[i + 2, 2].Value = statisticTables[i].CarID;
                        worksheet.Cells[i + 2, 3].Value = statisticTables[i].MakeName;
                        worksheet.Cells[i + 2, 4].Value = statisticTables[i].StaffID;
                        worksheet.Cells[i + 2, 5].Value = statisticTables[i].Date.ToString("yyyy-MM-dd");
                        worksheet.Cells[i + 2, 6].Value = statisticTables[i].Quantity;
                        worksheet.Cells[i + 2, 7].Value = statisticTables[i].Price;
                        worksheet.Cells[i + 2, 8].Value = statisticTables[i].Total;
                    }


                    var memoryStream = new MemoryStream();
                    await package.SaveAsAsync(memoryStream);
                    memoryStream.Position = 0;

                    return memoryStream;

                
                }
        }



        public async Task<IEnumerable<StatisticMake>> GetStatisticMakes()
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddMonths(-11);
            Dictionary<int, int> quantityMake = new Dictionary<int, int>();
            Dictionary<int, double> revenueMake = new Dictionary<int, double>();


            List<Order> orders = await _db.Orders
                .Include(o => o.DetailOrders)
                .ThenInclude(deo => deo.Car)
                .ThenInclude(c => c.Make)
                .Where(o => o.Status == "Đã thanh toán"
                 && (o.Date.Year > startTime.Year
                    || (o.Date.Year == startTime.Year && o.Date.Month >= startTime.Month))
                 && (o.Date.Year < endTime.Year
                    || (o.Date.Year == endTime.Year && o.Date.Month <= endTime.Month)))
                .ToListAsync();
            foreach (var order in orders)
            {
                foreach (var detailOrder in order.DetailOrders)
                {
                    if (!quantityMake.ContainsKey(detailOrder.Car.MakeID))
                    {
                        quantityMake[detailOrder.Car.MakeID] = 0;
                    }
                    quantityMake[detailOrder.Car.MakeID] += detailOrder.Quantity;

                    if (!revenueMake.ContainsKey(detailOrder.Car.MakeID))
                    {
                        revenueMake[detailOrder.Car.MakeID] = 0;
                    }
                    revenueMake[detailOrder.Car.MakeID] += detailOrder.Quantity * detailOrder.Price;


                }
            }

            List<Make> makes = await _db.Makes.ToListAsync();
            List<StatisticMake> statisticMakes = new List<StatisticMake>();
            foreach (var make in makes)
            {
                statisticMakes.Add(new StatisticMake
                {
                    MakeName = make.MakeName,
                    Quantity = quantityMake.ContainsKey(make.MakeID) ? quantityMake[make.MakeID] : 0,
                    Revenue = revenueMake.ContainsKey(make.MakeID) ? revenueMake[make.MakeID] / 1000000000 : 0
                });
            }
            return statisticMakes;
        }

        public async Task<IEnumerable<StatisticRevenue>> GetStatisticMonths()
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddMonths(-11);
            Dictionary<string, int> quantityMonth = new Dictionary<string, int>();
            Dictionary<string, double> revenueMonth = new Dictionary<string, double>();

            List<Order> orders = await _db.Orders
                .Include(o => o.DetailOrders)
                .Where(o => o.Status == "Đã thanh toán"
                && (o.Date.Year > startTime.Year 
                    || (o.Date.Year == startTime.Year && o.Date.Month >= startTime.Month))
                && (o.Date.Year < endTime.Year 
                    || (o.Date.Year == endTime.Year && o.Date.Month <= endTime.Month)))
                .ToListAsync();



            foreach (var order in orders)
            {
                foreach (var detailOrder in order.DetailOrders)
                {
                   
                    string timeKey = $"{order.Date.Month}/{order.Date.Year}";
                    if (!quantityMonth.ContainsKey(timeKey))
                    {
                        quantityMonth[timeKey] = 0;
                    }
                    quantityMonth[timeKey] += detailOrder.Quantity;

                    if (!revenueMonth.ContainsKey(timeKey))
                    {
                        revenueMonth[timeKey] = 0;
                    }
                    revenueMonth[timeKey] += detailOrder.Quantity * detailOrder.Price;
                }
            }
            List<StatisticRevenue> statisticRevenues = new List<StatisticRevenue>();
            for (int i = 0; i < 12; i++)
            {
                string timeKey = $"{startTime.Month}/{startTime.Year}";
                statisticRevenues.Add(new StatisticRevenue
                {
                    Month = timeKey,
                    Quantity = quantityMonth.ContainsKey(timeKey) ? quantityMonth[timeKey] : 0,
                    Revenue = revenueMonth.ContainsKey(timeKey) ? revenueMonth[timeKey] / 1000000000 : 0
                });
                startTime = startTime.AddMonths(1);
            }
            return statisticRevenues;
        }

        public async Task<IEnumerable<CarDTO>> GetBestCars()
        {
            List<Order> orders = await _db.Orders
                .Include(o => o.DetailOrders)
                .ThenInclude(deo => deo.Car)
                .ThenInclude(c => c.Make)
                .Where(o => o.Status == "Đã thanh toán")
                .ToListAsync();
            Dictionary<string, int> quantity = new Dictionary<string, int>();
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
            var sortedDict = quantity.OrderBy(q => q.Value).Take(4).ToList();
            List<CarDTO> cars = new List<CarDTO>();
            foreach (var item in sortedDict)
            {
                Car? _car = await _db.Cars.Include(c => c.Make).FirstOrDefaultAsync(c => c.CarID == item.Key);


                cars.Add(new CarDTO
                {
                    CarID = item.Key,
                    CarName = _car.CarName,
                    MakeName = _car.Make.MakeName,
                    Price = _car.Price,
                    Photo = _car.Photo
                });


            }
            return cars;
        }
        public async Task<IEnumerable<FeedBackHomeDTO>> GetBestFeedBacks()
        {
            List<Feedback> _feedBacks = await _db.Feedbacks.Include(fb => fb.User).Take(5).ToListAsync();
            List<FeedBackHomeDTO> feedBacks = new List<FeedBackHomeDTO>();


            foreach (var item in _feedBacks)
            {
                User? user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == item.UserID);

                feedBacks.Add(new FeedBackHomeDTO
                {
                    FullName = item.FullName,
                    Title = item.Title,
                    Content = item.Content,
                    Photo = (user != null) ? user.Photo : string.Empty
                });
            }
            return feedBacks;
        }
        public async Task<IEnumerable<NewsDTO>> GetBestNews()
        {
            List<News> _news = await _db.NewS.Include(n => n.Staff).ThenInclude(s => s.User).OrderByDescending(s => s.CreateAt).Take(3).ToListAsync();
            List<NewsDTO> news = new List<NewsDTO>();
            foreach (var item in _news)
            {
                news.Add(new NewsDTO
                {
                    NewsID = item.NewsID,
                    Photo = item.Photo,
                    FullName = item.Staff.User.FullName,
                    Title = item.Title,
                    CreateAt = item.CreateAt,
                });
            }
            return news;
        }

        public async Task<IEnumerable<StatisticTable>> GetStatisticTables(DateOnly? _startTime, DateOnly? _endTime, string? maNV, string? maXe, string? hangXe)
        {
            List<Order> orders = await _db.Orders
                .Include(o => o.DetailOrders)
                .ThenInclude(deo => deo.Car)
                .ThenInclude(c => c.Make)
                .Where(o => o.Status == "Đã thanh toán"
                && ((_startTime == null && _endTime == null)
                || (_startTime == null && _endTime != null && DateOnly.FromDateTime(o.Date) <= _endTime)
                || (_startTime != null && _endTime == null && _startTime <= DateOnly.FromDateTime(o.Date))
                || (_startTime != null && _endTime != null && _startTime <= DateOnly.FromDateTime(o.Date) && DateOnly.FromDateTime(o.Date) <= _endTime)))
                .ToListAsync();
            int cnt = 0;
            List<StatisticTable> statisticTables = new List<StatisticTable>();
            foreach (var order in orders)
            {
                foreach (var detailOrder in order.DetailOrders)
                {
                    if (maNV == null || (order.StaffID.ToLower().Contains(maNV.ToLower())))
                    {
                        if (hangXe == null || (detailOrder.Car.Make.MakeName.ToLower().Contains(hangXe.ToLower())))
                        {
                            if (maXe == null || (detailOrder.CarID.ToLower().Contains(maXe.ToLower())))
                            {
                                statisticTables.Add(new StatisticTable
                                {
                                    STT = ++cnt,
                                    CarID = detailOrder.CarID,
                                    MakeName = detailOrder.Car.Make.MakeName,
                                    StaffID = order.StaffID,
                                    Date = order.Date,
                                    Quantity = detailOrder.Quantity,
                                    Price = detailOrder.Price,
                                    Total = detailOrder.Quantity * detailOrder.Price

                                });
                            }
                        }
                    }

                }
            }
            return statisticTables;
        }
    }
}
