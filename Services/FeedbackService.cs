using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FeedbackService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _context = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateFeedback(FeedbackDTO feedback)
        {
            User? u = _context.Users
                 .Include(a => a.Account)
                 .FirstOrDefault(u => u.Account.Email == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
            if (u == null)
            {
                throw new Exception("Not found");
            }
                var feedback_id = 1;
                var lastFeedback = _context.Feedbacks.OrderByDescending(f => f.FeedbackID).FirstOrDefault();
                if (lastFeedback != null)
                {
                    feedback_id = Convert.ToInt32(lastFeedback.FeedbackID.Substring(2)) + 1;
                }
                var feedbackID = "PH" + feedback_id.ToString().PadLeft(6, '0');
                Feedback fe = new Feedback
                {
                    FeedbackID = feedbackID,
                    FullName = feedback.FullName,
                    Email = feedback.Email,
                    Title = feedback.Title,
                    Content = feedback.Content,
                    Rating = feedback.Rating,
                    Status = "Chưa xem",
                    CreateAt = DateTime.Now,
                    UserID = u.UserID,
                };
                _context.Feedbacks.Add(fe);
                await _context.SaveChangesAsync();
        }

        public async Task<Feedback> GetFeedbackById(string id)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(u => u.FeedbackID == id);
            if (feedback != null&&feedback.Status=="Chưa xem")
            {
                feedback.Status = "Đã xem";
                _context.Feedbacks.Update(feedback);
                await _context.SaveChangesAsync();
            }
            return feedback;
        }

        public async Task<List<Feedback>> GetFeedbacks(DateTime? date = null, string title = "")
        {
            List<Feedback> feedbacks =
                await _context.Feedbacks
                .Where(f => (date == null || f.CreateAt.Date == date)
                            && (string.IsNullOrEmpty(title) || f.Title.Contains(title)))
                .ToListAsync();
            return feedbacks;
        }

        public async Task RemoveFeedback(string id)
        {
            Feedback o = _context.Feedbacks.FirstOrDefault(f => f.FeedbackID == id);
            if (o != null)
            {
                _context.Feedbacks.Remove(o);
                await _context.SaveChangesAsync();
            }
        }
    }
}
