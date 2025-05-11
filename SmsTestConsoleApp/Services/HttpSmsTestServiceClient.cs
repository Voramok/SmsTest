using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using SmsTestConsoleApp.Interfaces;
using SmsTestService = SmsTestHttpLib.Services.SmsTestService;

namespace SmsTestConsoleApp.Services
{
    public class HttpSmsTestServiceClient : ISmsTestServiceClient
    {
        private readonly SmsTestService smsTestService;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpSmsTestServiceClient(string baseAdress, string userName, string password)
        {
             smsTestService = new SmsTestService(baseAdress, userName, password);
        }

        public async Task<Models.GetMenuResponse> GetMenuAsync(bool isActive)
        {
            try
            {
                var tempResponse = await smsTestService.GetMenuAsync(isActive);

                var response = new Models.GetMenuResponse
                {
                    Success = tempResponse.Success,
                    ErrorMessage = tempResponse.ErrorMessage,
                    MenuItems = tempResponse.Data.ToList().Select(tempMenuItem => new Models.MenuItem
                    {
                        Id = tempMenuItem.Id,
                        Article = tempMenuItem.Article,
                        Name = tempMenuItem.Name,
                        Price = tempMenuItem.Price,
                        IsWeighted = tempMenuItem.IsWeighted,
                        FullPath = tempMenuItem.FullPath,
                        Barcodes = new List<string>(tempMenuItem.Barcodes)
                    }).ToList()
                };

                return response;
            }
            catch (HttpRequestException ex)
            {
                logger.Error(ex, "Error fetching menu with active status {isActive}", isActive);
                throw; 
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while fetching the menu.");
                throw; 
            }
        }

        public async Task<Models.SendOrderResponse> SendOrderAsync(Models.Order order)
        {
            try
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order), "Order cannot be null.");
                }

                var tempOrder = new SmsTestHttpLib.Models.Order
                {
                    OrderId = order.Id
                };

                if (order.MenuItems != null)
                {
                    foreach (var item in order.MenuItems)
                    {
                        tempOrder.MenuItems.Add(new SmsTestHttpLib.Models.OrderItem
                        {
                            Id = item.Id,
                            Quantity = item.Quantity
                        });
                    }
                }

                var tempResponse = await smsTestService.PostOrderAsync(tempOrder);

                return new Models.SendOrderResponse
                {
                    Success = tempResponse.Success,
                    ErrorMessage = tempResponse.ErrorMessage,
                };
            }
            catch (HttpRequestException ex)
            {
                logger.Error(ex, "Error sending order: {@Order}", order);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while sending the order.");
                throw;
            }
        }
    }
}
