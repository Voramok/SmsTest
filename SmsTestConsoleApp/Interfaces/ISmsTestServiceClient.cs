using System.Threading.Tasks;
using SmsTestConsoleApp.Models;

namespace SmsTestConsoleApp.Interfaces
{
    public interface ISmsTestServiceClient
    {
        Task<GetMenuResponse> GetMenuAsync(bool isActive);
        Task<SendOrderResponse> SendOrderAsync(Order order);
    }
}
