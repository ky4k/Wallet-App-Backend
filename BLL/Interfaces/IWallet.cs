using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Interfaces
{
    public interface IWallet
    {
        Task<UserDto> GetUserWithBalanceAsync(Guid userId);
        Task<IEnumerable<TransactionDto>> GetLatestTransactionsAsync(Guid userId, int count);
        Task<TransactionDto> GetTransactionDetailsAsync(Guid transactionId);
        Task<string> CalculateDailyPointsAsync(Guid userId, DateTime date);
    }
}
