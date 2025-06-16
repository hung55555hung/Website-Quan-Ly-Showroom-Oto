
using Microsoft.IdentityModel.Tokens;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class PhotoService : IPhotoService
    {
        private IWebHostEnvironment _environment;
        public PhotoService (IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> AddPhoto(string directory, IFormFile uploadimage)
        {
            try
            {
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(uploadimage!.FileName);
                string imageFullPath = Path.Combine(_environment.WebRootPath, $"upload\\{directory}", newFileName);
                using (var fileStream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await uploadimage.CopyToAsync(fileStream);

                }
                return newFileName;
            } catch (Exception ex)
            {
                throw new Exception("Lỗi trong khi thêm ảnh: ", ex);
            }


        }

        public Task<string> EditPhoto(string directory, IFormFile uploadimage, string oldfilename)
        {
            if (oldfilename.IsNullOrEmpty() == false)
            {
                string oldImageFullPath = Path.Combine(_environment.WebRootPath, $"upload\\{directory}", oldfilename);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }
            return AddPhoto(directory, uploadimage);
        }
    }
}
