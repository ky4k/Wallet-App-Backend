using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = DAL.Models.Transaction;

namespace BLL.Services
{
    public class WalletService : IWallet
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        public WalletService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<string> CalculateDailyPointsAsync(Guid userId, DateTime date)
        {
            // Отримуємо користувача
            var user = await _transactionRepository.GetUserWithBalanceAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (user.LastPointsDate.Date == date.Date)
            {
                return FormatPoints(user.Points); 
            }

            int dayOfSeason = (date - GetSeasonStartDate(date)).Days + 1;

            int points = 2; 
            if (dayOfSeason > 1) points += 1; 

            int previousDayPoints = 3;
            int twoDaysAgoPoints = 2;

            for (int i = 3; i <= dayOfSeason; i++)
            {
                points = (int)Math.Round(twoDaysAgoPoints * 1.0 + previousDayPoints * 0.6);
                twoDaysAgoPoints = previousDayPoints;
                previousDayPoints = points;
            }

            user.LastPointsDate = date;
            user.Points = points;

            await _transactionRepository.UpdateUserAsync(user);

            return FormatPoints(points); 
        }

        private string FormatPoints(int points)
        {
            return points >= 1000 ? $"{points / 1000}K" : points.ToString();
        }

        private DateTime GetSeasonStartDate(DateTime date)
        {
            return new DateTime(date.Year, 1, 1); 
        }

        public async Task<IEnumerable<TransactionDto>> GetLatestTransactionsAsync(Guid userId, int count = 10)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero.", nameof(count));

            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId, count);

            if (transactions == null || !transactions.Any())
                throw new KeyNotFoundException("No transactions found for this user.");

            var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

            return transactionDtos;
        }
        public async Task<TransactionDto> GetTransactionDetailsAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found.");

            return _mapper.Map<TransactionDto>(transaction);
        }
        public async Task<UserDto> GetUserWithBalanceAsync(Guid userId)
        {
            var user = await _transactionRepository.GetUserWithBalanceAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

    }
}
