using System;
using System.Collections.Generic;

namespace SmsTestConsoleApp.Models
{
    public sealed class GetMenuResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<MenuItem> MenuItems { get; set; }

        public GetMenuResponse()
        {
            MenuItems = new List<MenuItem>();
        }
    }
}
