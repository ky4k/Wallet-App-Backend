using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required User User { get; set; }
        public string Type { get; set; } = string.Empty; // Payment or Credit
        public decimal Amount { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsPending { get; set; }
        public string AuthorizedUser { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
    }
}
