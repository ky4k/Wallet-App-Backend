using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId, int count);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId);
        Task<User> GetUserWithBalanceAsync(Guid userId);
        Task UpdateUserAsync(User user);
    }
}
