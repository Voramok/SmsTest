using System.ComponentModel.DataAnnotations;

namespace SmsTestHttpLib.Models
{
    public class BaseResponse
    {
        [Required(ErrorMessage = "Command является обязательным полем.")]
        public required string Command { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
