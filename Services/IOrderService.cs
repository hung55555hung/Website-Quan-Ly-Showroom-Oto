using WebPBL3.DTO;
using WebPBL3.Models;

namespace WebPBL3.Services
{
    public interface IOrderService
    {

        public Task<List<Order>> GetOrders(string status, string idUser);
        public Task CreateOrder(OrderDTO order);

        public Task<DetailOrderDTO> GetDetailOrder(string id);
        public Task UpdateOrder(string id);

    
        public Task<OrderDTO> getInforByEmail(string  email);
        
        public Task DeleteOrder(string id);
      
    }
}
