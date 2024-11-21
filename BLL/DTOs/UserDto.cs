using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CardBalance { get; set; }
        public decimal CardLimit { get; set; } = 1500m;
        public decimal AvailableBalance => CardLimit - CardBalance;
        public string PaymentStatus => $"You’ve paid your {DateTime.Now.ToString("MMMM")} balance";

    }
}
