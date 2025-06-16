using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebPBL3.DTO;
using WebPBL3.Models;

/*namespace WebPBL3.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _enviroment;
        public NewsService(ApplicationDbContext db, IWebHostEnvironment enviroment)
        {
            _db = db;
            _enviroment = enviroment;
        }
        public async Task AddNews(News news)
        {
            try
            {
                _db.NewS.Add(news);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi thêm tin tức: ", ex);
            }
        }

        public News ConvertToNews(NewsDTO newsdto)
        {
            try
            {
                News news = new News
                {
                    NewsID = newsdto.NewsID,
                    Title = newsdto.Title,
                    Content = newsdto.Content,
                    Photo = newsdto.Photo,
                    CreateAt = newsdto.CreateAt,
                    UpdateAt = newsdto.UpdateAt,
                    UpdateBy = newsdto.UpdateBy,
                    StaffID = newsdto.StaffID,
                };
                return news;
            }
            catch(Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi chuyển đổi NewsDTO thành News: ", ex);
            }
        }

        public NewsDTO ConvertToNewsDTO(News news)
        {
            try
            {
                NewsDTO newsdto = new NewsDTO
                {
                    NewsID = news.NewsID,
                    Title = news.Title,
                    Content = news.Content,
                    Photo = news.Photo,
                    CreateAt = news.CreateAt,
                    UpdateAt = news.UpdateAt,
                    UpdateBy = news.UpdateBy,
                    StaffID = news.StaffID,
                };
                return newsdto;
            }
            catch(Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi chuyển News thành NewsDTO: ", ex);
            }
        }

        public async Task DeleteNews(News news)
        {
            try
            {
                _db.NewS.Remove(news);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi xóa tin tức: ", ex);
            }
        }

        public async Task EditNews(News news)
        {
            try
            {
                _db.NewS.Update(news);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi cập nhật tin tức: ", ex);
            }
        }

        public async Task<IEnumerable<NewsDTO>> GetAllNews(int makeid, string searchtxt, int page)
        {
            try
            {
                List<NewsDTO> news = await _db.NewS
                    .Include(s => s.Staff)
                    .ThenInclude(u => u.User)
                    .Select(n => new NewsDTO
                     {
                        NewsID = n.NewsID,
                        Title = n.Title,
                        Content = n.Content,
                        Photo = n.Photo,
                        CreateAt = n.CreateAt,
                        UpdateAt = n.UpdateAt,
                        UpdateBy = n.UpdateBy,
                        StaffID = n.StaffID,
                        FullName = n.Staff.User.FullName,
                     }).ToListAsync();
                    return news;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi lấy danh sách tin tức: ", ex);
            }
        }

        public async Task<News> GetNewsById(string id)
        {
            try
            {
                News? news = await _db.NewS.FirstOrDefaultAsync(u => u.NewsID == id);
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi try vấn tin tức: ", ex);
            }
        }
    }
}
*/

namespace WebPBL3.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public NewsService(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task<List<NewsDTO>> GetNewsAsync()
        {
            return await _db.NewS.Include(s => s.Staff)
                                 .ThenInclude(u => u.User)
                                 .Select(n => new NewsDTO
                                 {
                                     NewsID = n.NewsID,
                                     Title = n.Title,
                                     Content = n.Content,
                                     Photo = n.Photo,
                                     CreateAt = n.CreateAt,
                                     UpdateAt = n.UpdateAt,
                                     UpdateBy = n.UpdateBy,
                                     StaffID = n.StaffID,
                                     FullName = n.Staff.User.FullName,
                                 }).ToListAsync();
        }
        public async Task<List<News>> GetAllNews()
        {
            return await _db.NewS.ToListAsync();
        }

        //lấy tin tức theo id
        public async Task<News> GetNewsById(string id)
        {
            try
            {
                News? news = await _db.NewS.FirstOrDefaultAsync(u => u.NewsID == id);
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi try vấn tin tức: ", ex);
            }
        }

        // Chuyển News thành NewsDTO
        public NewsDTO ConvertToNewsDTO(News news)
        {
            try
            {
                NewsDTO newsdto = new NewsDTO
                {
                    NewsID = news.NewsID,
                    Title = news.Title,
                    Content = news.Content,
                    Photo = news.Photo,
                    CreateAt = news.CreateAt,
                    UpdateAt = news.UpdateAt,
                    UpdateBy = news.UpdateBy,
                    StaffID = news.StaffID,
                };
                return newsdto;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi chuyển News thành NewsDTO: ", ex);
            }
        }

        // Chuyển NewsDTO thành News
        public News ConvertToNews(NewsDTO newsdto)
        {
            try
            {
                News news = new News
                {
                    NewsID = newsdto.NewsID,
                    Title = newsdto.Title,
                    Content = newsdto.Content,
                    Photo = newsdto.Photo,
                    CreateAt = newsdto.CreateAt,
                    UpdateAt = newsdto.UpdateAt,
                    UpdateBy = newsdto.UpdateBy,
                    StaffID = newsdto.StaffID,
                };
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất hiện khi chuyển đổi NewsDTO thành News: ", ex);
            }
        }
        public async Task<PaginatedList<NewsDTO>> GetPaginatedNewsAsync(int page, int pageSize, string searchtxt, string exactDate, string startDate, string endDate)
        {
            var newsQuery = _db.NewS.Include(s => s.Staff)
                                    .ThenInclude(u => u.User)
                                    .Select(n => new NewsDTO
                                    {
                                        NewsID = n.NewsID,
                                        Title = n.Title,
                                        Content = n.Content,
                                        Photo = n.Photo,
                                        CreateAt = n.CreateAt,
                                        UpdateAt = n.UpdateAt,
                                        UpdateBy = n.UpdateBy,
                                        StaffID = n.StaffID,
                                        FullName = n.Staff.User.FullName,
                                    });
                                   

            if (!string.IsNullOrWhiteSpace(searchtxt))
            {
                newsQuery = newsQuery.Where(n => n.FullName.Contains(searchtxt));
            }

            DateTime parsedExactDate;
            DateTime parsedStartDate;
            DateTime parsedEndDate;

            if (!string.IsNullOrWhiteSpace(exactDate) && DateTime.TryParse(exactDate, out parsedExactDate))
            {
                newsQuery = newsQuery.Where(n => n.CreateAt.Date == parsedExactDate.Date);
            }
            else if (DateTime.TryParse(startDate, out parsedStartDate) && DateTime.TryParse(endDate, out parsedEndDate))
            {
                newsQuery = newsQuery.Where(n => n.CreateAt.Date >= parsedStartDate.Date && n.CreateAt.Date <= parsedEndDate.Date);
            }
           
            return await PaginatedList<NewsDTO>.CreateAsync(newsQuery, page, pageSize);
        }


        public async Task<NewsDTO> CreateNewsAsync(NewsDTO news, IFormFile uploadImage, string userEmail, string FILENAME)
        {
            var newsid = 1;
            var lastNews = await _db.NewS.OrderByDescending(n => n.NewsID).FirstOrDefaultAsync();
            if (lastNews != null)
            {
                newsid = Convert.ToInt32(lastNews.NewsID.Substring(2)) + 1;
            }
            var newsidTxt = "TT" + newsid.ToString().PadLeft(6, '0');
            news.NewsID = newsidTxt;

            
            news.Photo = FILENAME;

            var account = await _db.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Email == userEmail);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            string userid = account.User.UserID;
            var staff = await _db.Staffs.FirstOrDefaultAsync(a => a.UserID == userid);
            if (staff == null)
            {
                throw new Exception("Staff not found");
            }

            var newNews = new News
            {
                NewsID = news.NewsID,
                Title = news.Title,
                Content = news.Content,
                Photo = news.Photo,
                CreateAt = DateTime.Now,
                UpdateAt = null,
                StaffID = staff.StaffID,
            };

            _db.NewS.Add(newNews);
            await _db.SaveChangesAsync();

            return news;
        }

        public async Task<NewsDTO> GetNewsByIdAsync(string id)
        {
            var news = await _db.NewS.Include(n => n.Staff).ThenInclude(s => s.User)
                                     .FirstOrDefaultAsync(n => n.NewsID == id);
            if (news == null)
            {
                return null;
            }

            return new NewsDTO
            {
                NewsID = news.NewsID,
                Title = news.Title,
                Content = news.Content,
                Photo = news.Photo,
                CreateAt = news.CreateAt,
                UpdateAt = news.UpdateAt,
                StaffID = news.StaffID,
                FullName = news.Staff.User.FullName,
            };
        }

        public async Task UpdateNewsAsync(NewsDTO n, string userEmail, string FILENAME, string? id)
        {
            News? news = _db.NewS.Find(id);
            if (FILENAME != "")
            {
                news.Photo = FILENAME;
            }        
            Account? account = await _db.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Email == userEmail);
            if (account == null)
            {
                throw new Exception("Lỗi khi truy vấn tài khoản");
            }
            string userid = account.User.UserID;
            Staff? staff = await _db.Staffs.FirstOrDefaultAsync(a => a.UserID == userid);
            string staffid = staff.StaffID;

            news.Title = n.Title;
            news.Content = n.Content;
            news.CreateAt = n.CreateAt;
            news.UpdateAt = DateTime.Now;
            news.UpdateBy = staffid;
            news.StaffID = news.StaffID;

            _db.NewS.Update(news);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteNewsAsync(string id)
        {
            var news = await _db.NewS.FindAsync(id);
            if (news == null)
            {
                throw new Exception("News not found");
            }

            _db.NewS.Remove(news);
            await _db.SaveChangesAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
            {
                throw new Exception("File is empty.");
            }

            var fileName = Path.GetFileName(upload.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "upload/news", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }
            return fileName;
        }
    }
}