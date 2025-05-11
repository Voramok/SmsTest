using System.ComponentModel.DataAnnotations;

namespace SmsTestHttpLib.Models
{
    public sealed class OrderItem
    {
        [Required(ErrorMessage = "Id является обязательным полем.")]
        public required string Id { get; set; }
        public double Quantity { get; set; }
    }
}
