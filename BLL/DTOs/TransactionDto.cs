using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty; // Payment or Credit
        public decimal Amount { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsPending { get; set; }
        public string AuthorizedUser { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty;
        public string DisplayDescription => IsPending ? $"Pending: {Description}" : Description;
    }
}
