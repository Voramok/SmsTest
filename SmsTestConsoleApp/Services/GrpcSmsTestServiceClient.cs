using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Sms.Test;
using SmsTestConsoleApp.Interfaces;

namespace SmsTestConsoleApp.Services
{
    public class GrpcSmsTestServiceClient : ISmsTestServiceClient
    {
        private readonly SmsTestService.SmsTestServiceClient _client;

        public GrpcSmsTestServiceClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new SmsTestService.SmsTestServiceClient(channel);
        }

        public async Task<Models.GetMenuResponse> GetMenuAsync(bool isActive)
        {
            var menuRequest = new BoolValue { Value = isActive };

            var tempResponse = await _client.GetMenuAsync(menuRequest);

            var response = new Models.GetMenuResponse
            {
                Success = tempResponse.Success,
                ErrorMessage = tempResponse.ErrorMessage,
                MenuItems = tempResponse.MenuItems.Select(tempMenuItem => new Models.MenuItem
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

        public async Task<Models.SendOrderResponse> SendOrderAsync(Models.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null.");
            }

            var tempOrder = new Order
            {
                Id = order.Id
            };

            if (order.MenuItems != null)
            {
                foreach (var item in order.MenuItems)
                {
                    tempOrder.OrderItems.Add(new OrderItem
                    {
                        Id = item.Id,
                        Quantity = item.Quantity
                    });
                }
            }

            var tempResponse = await _client.SendOrderAsync(tempOrder);

            return new Models.SendOrderResponse
            {
                Success = tempResponse.Success,
                ErrorMessage = tempResponse.ErrorMessage,
            };
        }
    }
}
