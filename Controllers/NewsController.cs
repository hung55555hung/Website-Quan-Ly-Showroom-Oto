using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebPBL3.DTO;
using WebPBL3.Models;
using WebPBL3.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebPBL3.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private int limits = 10;
        public NewsController(ApplicationDbContext db, IWebHostEnvironment environment, INewsService newsService)
        {
            _db = db;
            _environment = environment;
            _newsService = newsService;

        }
        public async Task<IActionResult> Index()
        {
           // List<News> list = _db.NewS.ToList();
            List<News> list = await _newsService.GetAllNews();
            ViewBag.HideHeader = false;
            return View(list);
        }

        /*[Authorize(Roles = "Admin, Staff")]
        public IActionResult ListNews(int newid = 0, string searchtxt = "", string exactDate = "", string startDate = "", string endDate = "", int page = 1)
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

            var newsList = newsQuery.ToList();

            if (!newsList.Any() && (!string.IsNullOrWhiteSpace(searchtxt) || !string.IsNullOrWhiteSpace(exactDate) || !string.IsNullOrWhiteSpace(startDate) || !string.IsNullOrWhiteSpace(endDate)))
            {
                ViewBag.Message = "Không có tin tức nào được tìm thấy";
            }

            var total = newsList.Count;
            var totalPage = (total + limits - 1) / limits;
            if (page < 1) page = 1;
            if (page > totalPage) page = totalPage;
            ViewBag.totalRecord = total;
            ViewBag.totalPage = totalPage;
            ViewBag.currentPage = page;
            ViewBag.searchtxt = searchtxt;
            ViewBag.newid = newid;
            ViewBag.exactDate = exactDate;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            var paginatedNews = newsList.Skip((page - 1) * limits).Take(limits).ToList();

            int cnt = 1;
            foreach (var n in paginatedNews)
            {
                n.STT = cnt++;
            }

            return View(paginatedNews);
        }*/
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> ListNews(int newid = 0, string searchtxt = "", string exactDate = "", string startDate = "", string endDate = "", int page = 1)
        {
            int pageSize = 10;
            var paginatedNews = await _newsService.GetPaginatedNewsAsync(page, pageSize, searchtxt, exactDate, startDate, endDate);

            ViewBag.totalRecord = paginatedNews.Count;
            ViewBag.totalPage = paginatedNews.TotalPages;
            ViewBag.currentPage = page;
            ViewBag.searchtxt = searchtxt;
            ViewBag.newid = newid;
            ViewBag.exactDate = exactDate;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            int cnt = paginatedNews.Count();
            foreach (var n in paginatedNews)
            {
                n.STT = cnt--;
            }
            return View(paginatedNews);
        }

        //Get
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Create()
        {
            return View();
        }
        //Post
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(NewsDTO news, IFormFile uploadimage)
        {
            /*  if (!ModelState.IsValid)
              {
                  try
                  {
                      var newsid = 1;
                      var lastNews = _db.NewS.OrderByDescending(n => n.NewsID).FirstOrDefault();
                      if (lastNews != null)
                      {
                          newsid = Convert.ToInt32(lastNews.NewsID.Substring(2)) + 1;
                      }
                      var newsidTxt = "TT" + newsid.ToString().PadLeft(6, '0');
                      news.NewsID = newsidTxt;
                      string FILENAME = "";
                      if (TempData["UploadedFileName"] != null)
                      {
                          FILENAME = TempData["UploadedFileName"].ToString();
                      }
                      news.Photo = FILENAME;
                      // lấy staffid
                      if (!User.Identity.IsAuthenticated)
                      {
                          return BadRequest("Người dùng chưa đăng nhập");
                      }
                      string? email = User.Identity.Name;
                      Account? account = await _db.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Email == email);
                      if (account == null)
                      {
                          return NotFound("Account is not found");
                      }
                      string userid = account.User.UserID;
                      Staff? staff = await _db.Staffs.FirstOrDefaultAsync(a => a.UserID == userid);
                      string staffid = staff.StaffID;

                      _db.NewS.Add(new News
                      {
                          NewsID = news.NewsID,
                          Title = news.Title,
                          Content = news.Content,
                          Photo = news.Photo,
                          CreateAt = DateTime.Now,
                          UpdateAt = null,
                          StaffID = staffid,
                      });
                      await _db.SaveChangesAsync();
                      return RedirectToAction("ListNews");
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine(ex.InnerException?.Message);
                      ModelState.AddModelError(string.Empty, "An error occurred while saving changes. Please try again.");
                  }
              }
              return View(news);*/

            if (!ModelState.IsValid)
            {
                try
                {
                    string userEmail = User.Identity.Name;
                    string FILENAME = "";
                    if (TempData["UploadedFileName"] != null)
                    {
                        FILENAME = TempData["UploadedFileName"].ToString();
                    }
                    if (string.IsNullOrEmpty(FILENAME))
                    {
                        ModelState.AddModelError(string.Empty, "Ảnh không thể trống");
                        return View(news);
                    }
                    TempData.Remove("UploadedFileName");
                    await _newsService.CreateNewsAsync(news, uploadimage, userEmail, FILENAME);
                    return RedirectToAction("ListNews");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "");
                }
            }
            return View(news);
        }
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            //   News? news = _db.NewS.Find(id);
            News? news = await _newsService.GetNewsById(id);
            if (news == null)
            {
                return NotFound();
            }
            /* NewsDTO NewsDTOFromDb = new NewsDTO
             {
                 NewsID = news.NewsID,
                 Title = news.Title,
                 Content = news.Content,
                 Photo = news.Photo,
                 CreateAt = news.CreateAt,
                 UpdateAt = news.UpdateAt,
                 StaffID = "1",
             };*/
            NewsDTO NewsDTOFromDb = _newsService.ConvertToNewsDTO(news);
            var url = $"{Request.Scheme}://{Request.Host}/upload/news/{news.Photo}";
            ViewBag.filePath = url;
            return View(NewsDTOFromDb);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(NewsDTO n, IFormFile? uploadimage, string? id)
        {
            /* if (!ModelState.IsValid)
             {
                 try
                 {
                     News? news = _db.NewS.Find(id);
                     bool ok = false;
                     string FILENAME = "";
                     if (TempData["UploadedFileName"] != null)
                     {
                         FILENAME = TempData["UploadedFileName"].ToString();
                         ok = true;
                     }
                     if (ok == true)
                         news.Photo = FILENAME;
                     else
                         news.Photo = n.Photo;
                     // lấy staffid
                     if (!User.Identity.IsAuthenticated)
                     {
                         return BadRequest("Người dùng chưa đăng nhập");
                     }
                     string? email = User.Identity.Name;
                     Account? account = await _db.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Email == email);
                     if (account == null)
                     {
                         return NotFound("Account is not found");
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

                     return RedirectToAction("ListNews");
                 }
                 catch(Exception ex)
                 {
                     Console.WriteLine(ex.InnerException?.Message);
                     ModelState.AddModelError(string.Empty, "An error occurred while saving changes. Please try again.");
                 }
             }
             return View(n);*/

            if (!ModelState.IsValid)
            {
                try
                {
                    string FILENAME = "";
                    if (TempData["UploadedFileName"] != null)
                    {
                        FILENAME = TempData["UploadedFileName"].ToString();
                    }
                    if (!User.Identity.IsAuthenticated)
                    {
                        return BadRequest("Người dùng chưa đăng nhập");
                    }
                    string userEmail = User.Identity.Name;
                    TempData.Remove("UploadedFileName");
                    await _newsService.UpdateNewsAsync(n, userEmail, FILENAME, id);
                    return RedirectToAction("ListNews");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "");
                }
            }
            return View(n);
        }

        public async Task<IActionResult> DetailUser(string? id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            // News? news = _db.NewS.Find(id);
            News? news = await _newsService.GetNewsById(id);
            if (news == null)
            {
                return NotFound();
            }
            /* NewsDTO NewsDTOFromDb = new NewsDTO
             {
                 NewsID = news.NewsID,
                 Title = news.Title,
                 Content = news.Content,
                 Photo = news.Photo,
                 CreateAt = news.CreateAt,
                 UpdateAt = DateTime.Now,
                 StaffID = news.StaffID,
             };*/
            NewsDTO NewsDTOFromDb = _newsService.ConvertToNewsDTO(news);
            //  List<News> _news = _db.NewS.ToList();
            List<News> _news = await _newsService.GetAllNews();
            ViewBag._news = _news;
            ViewBag.HideHeader = true;
            return View(NewsDTOFromDb);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            /* if (String.IsNullOrEmpty(id))
             {
                 return NotFound();
             }
             News? newsToDelete = _db.NewS.FirstOrDefault(n => n.NewsID == id);

             if (newsToDelete == null)
             {
                 return NotFound();
             }
             try
             {
                 _db.NewS.Remove(newsToDelete);
                 await _db.SaveChangesAsync();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
             }
             return RedirectToAction("ListNews");*/

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                await _newsService.DeleteNewsAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("lỗi lưu");
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("ListNews");
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            /* if (upload == null || upload.Length == 0)
             {
                 return BadRequest("File is empty.");
             }
             var fileName = Path.GetFileName(upload.FileName);
             var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
             TempData["UploadedFileName"] = fileName;
             Directory.CreateDirectory(Path.GetDirectoryName(filePath));
             using (var stream = new FileStream(filePath, FileMode.Create))
             {
                 await upload.CopyToAsync(stream);
             }
             var url = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
             return Json(new { uploaded = true, url });*/

            try
            {
                string fileName = await _newsService.UploadImageAsync(upload);
                string url =  $"{Request.Scheme}://{Request.Host}/upload/news/{fileName}";
                TempData["UploadedFileName"] = fileName;
                return Json(new { uploaded = true, url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}



/*namespace WebPBL3.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index()
        {
            List<News> list = await _newsService.GetAllNews();
            ViewBag.HideHeader = false;
            return View(list);
        }

[Authorize(Roles = "Admin, Staff")]
public async Task<IActionResult> ListNews(int newid = 0, string searchtxt = "", string exactDate = "", string startDate = "", string endDate = "", int page = 1)
{
    int pageSize = 10;
    var paginatedNews = await _newsService.GetPaginatedNewsAsync(page, pageSize, searchtxt, exactDate, startDate, endDate);

    ViewBag.totalRecord = paginatedNews.Count;
    ViewBag.totalPage = paginatedNews.TotalPages;
    ViewBag.currentPage = page;
    ViewBag.searchtxt = searchtxt;
    ViewBag.newid = newid;
    ViewBag.exactDate = exactDate;
    ViewBag.startDate = startDate;
    ViewBag.endDate = endDate;

    return View(paginatedNews);
}


[Authorize(Roles = "Admin, Staff")]
public IActionResult Create()
{
    return View();
}

[Authorize(Roles = "Admin, Staff")]
[HttpPost]
        public async Task<IActionResult> Create(NewsDTO news, IFormFile uploadImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userEmail = User.Identity.Name;
                    await _newsService.CreateNewsAsync(news, uploadImage, userEmail);
                    return RedirectToAction("ListNews");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(news);
        }

        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            var url = $"{Request.Scheme}://{Request.Host}/images/{news.Photo}";
            ViewBag.filePath = url;
            return View(news);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Edit(NewsDTO news, IFormFile? uploadImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userEmail = User.Identity.Name;
                    await _newsService.UpdateNewsAsync(news, uploadImage, userEmail);
                    return RedirectToAction("ListNews");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(news);
        }

        public async Task<IActionResult> DetailUser(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            var allNews = await _newsService.GetNewsAsync();
            ViewBag._news = allNews;
            ViewBag.HideHeader = true;
            return View(news);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                await _newsService.DeleteNewsAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("ListNews");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            try
            {
                string url = await _newsService.UploadImageAsync(upload);
                return Json(new { uploaded = true, url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
*/

