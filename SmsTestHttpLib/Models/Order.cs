using System.ComponentModel.DataAnnotations;

namespace SmsTestHttpLib.Models
{
    public sealed class Order
    {
        [Required(ErrorMessage = "OrderId является обязательным полем.")]
        public string OrderId { get; set; }
        public List<OrderItem>? MenuItems { get; set; }

        public Order()
        {
            MenuItems = new List<OrderItem>();
        }
    }
}
