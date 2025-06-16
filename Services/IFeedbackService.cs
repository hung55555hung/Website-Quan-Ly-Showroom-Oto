using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface IFeedbackService
    {
        public Task CreateFeedback(FeedbackDTO feedbackDTO);
        public Task<List<Feedback>> GetFeedbacks(DateTime? date = null, string title = "");
        public Task<Feedback> GetFeedbackById(string id);

        public Task RemoveFeedback(string id);
    }
}
