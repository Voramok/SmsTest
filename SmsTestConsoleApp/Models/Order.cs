using System.Collections.Generic;

namespace SmsTestConsoleApp.Models
{
    public sealed class Order
    {
        public string Id { get; set; }
        public List<OrderItem> MenuItems { get; set; }

        public Order()
        {
            MenuItems = new List<OrderItem>();
        }
    }
}
