namespace WebPBL3.Services
{
    public interface IPhotoService
    {
        public Task<string> AddPhoto(string directory, IFormFile uploadimage);
        public Task<string> EditPhoto(string directory, IFormFile uploadimage, string filename);
        
    }
}
