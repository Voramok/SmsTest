using System.ComponentModel.DataAnnotations;

namespace SmsTestHttpLib.Models
{
    public sealed class MenuItem
    {
        [Required(ErrorMessage = "Id является обязательным полем.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Артикул является обязательным полем.")]
        [StringLength(30, ErrorMessage = "Артикул не должен превышать 30 символов.")]
        public required string Article { get; set; }

        [Required(ErrorMessage = "Название является обязательным полем.")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Цена является обязательным полем.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом.")]
        public double Price { get; set; }

        public bool IsWeighted { get; set; }

        [Required(ErrorMessage = "Путь является обязательным полем.")]
        public required string FullPath { get; set; }

        public List<string> Barcodes { get; set; } = new List<string>();

        public MenuItem()
        {
            Barcodes = new List<string>();
        }
    }
}
