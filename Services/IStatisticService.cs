using Microsoft.AspNetCore.Mvc;
using WebPBL3.DTO;
using WebPBL3.DTO.Statistic;

namespace WebPBL3.Services
{
    public interface IStatisticService
    {
        Task<int> CountUsers();
        Task<int> CountCars();
        Task<int> CountStaffs();
        Task<int> CountFeedBacks();
        Task<IEnumerable<StatisticMake>> GetStatisticMakes();
        Task<IEnumerable<StatisticRevenue>> GetStatisticMonths();
        Task<IEnumerable<StatisticTable>> GetStatisticTables(DateOnly? _startTime, DateOnly? _endTime, string? maNV, string? maXe, string? hangXe);
        Task<MemoryStream> CreateAndSaveExcel(List<StatisticTable> statisticTables);

        Task<IEnumerable<CarDTO>> GetBestCars();
        Task<IEnumerable<FeedBackHomeDTO>> GetBestFeedBacks();
        Task<IEnumerable<NewsDTO>> GetBestNews();
        Task<string> GetPhotoByEmail(string email);
    }
}
