using NuGet.Packaging.Signing;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface INewsService
    {
        /*Task<IEnumerable<NewsDTO>> GetAllNews(int newsid, string searchtxt, int page);
        Task<News> GetNewsById(string id);
        Task AddNews(News news);
        Task EditNews(News news);
        Task DeleteNews(News news);
        NewsDTO ConvertToNewsDTO(News news);
        News ConvertToNews(NewsDTO newsDTO);*/

            NewsDTO ConvertToNewsDTO(News news);
            News ConvertToNews(NewsDTO newsDTO);
         //   Task<List<NewsDTO>> GetNewsAsync();
            Task<List<News>> GetAllNews();
            Task<News> GetNewsById(string id);
            Task<PaginatedList<NewsDTO>> GetPaginatedNewsAsync(int page, int pageSize, string searchtxt, string exactDate, string startDate, string endDate);
            Task<NewsDTO> CreateNewsAsync(NewsDTO news, IFormFile uploadImage, string userEmail, string? fILENAME);
            Task<NewsDTO> GetNewsByIdAsync(string id);
            Task UpdateNewsAsync(NewsDTO news,string userEmail, string FILENAME, string? id);
            Task DeleteNewsAsync(string id);
            Task<string> UploadImageAsync(IFormFile upload);
      
    }
}
