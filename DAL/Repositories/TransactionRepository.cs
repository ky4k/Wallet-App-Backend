using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly WalletDbContext _context;
        public TransactionRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserWithBalanceAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId, int count)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
