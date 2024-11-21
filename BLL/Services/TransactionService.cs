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
            var user = await _transactionRepository.GetUserWithBalanceAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            date = date.ToUniversalTime();

            // Check if points have already been calculated for today
            if (user.LastPointsDate.Date == date.Date)
            {
                return FormatPoints(user.Points); // Return existing points if calculated today
            }

            // Calculate day of season
            int dayOfSeason = (date - GetSeasonStartDate(date)).Days + 1;

            // Initialize points based on the day of the season
            int points = 0;
            if (dayOfSeason == 1)
            {
                points = 2; // First day of season gets 2 points
            }
            else if (dayOfSeason == 2)
            {
                points = 3; // Second day of season gets 3 points
            }
            else if (dayOfSeason > 2)
            {
                int previousDayPoints = 3;
                int twoDaysAgoPoints = 2;

                // For subsequent days, calculate points based on previous values
                for (int i = 3; i <= dayOfSeason; i++)
                {
                    points = (int)Math.Round(twoDaysAgoPoints * 1.0 + previousDayPoints * 0.6);
                    twoDaysAgoPoints = previousDayPoints;
                    previousDayPoints = points;
                }
            }

            // Ensure points are valid (no negative points)
            if (points < 0)
            {
                points = 0; // Prevent negative points
            }

            // Update the user's points and last points date
            user.LastPointsDate = date;
            user.Points = points;

            // Save the changes to the database
            await _transactionRepository.UpdateUserAsync(user);

            return FormatPoints(points); // Return formatted points
        }

        private string FormatPoints(int points)
        {
            return points >= 1000 ? $"{points / 1000}K" : points.ToString();
        }

        private DateTime GetSeasonStartDate(DateTime date)
        {
            int year = date.Year;
            if (date.Month >= 3 && date.Month <= 5)
                return new DateTime(year, 3, 1); // Spring
            else if (date.Month >= 6 && date.Month <= 8)
                return new DateTime(year, 6, 1); // Summer
            else if (date.Month >= 9 && date.Month <= 11)
                return new DateTime(year, 9, 1); // Autumn
            else
                return new DateTime(year, 12, 1); // Winter
        }

        public async Task<IEnumerable<TransactionDto>> GetLatestTransactionsAsync(Guid userId, int count = 10)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero.", nameof(count));

            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId, count);

            if (transactions == null || !transactions.Any())
                throw new KeyNotFoundException("No transactions found for this user.");

            var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

            foreach (var transaction in transactionDtos)
            {
                transaction.IconUrl = GetTransactionIcon(transaction.Type); 
            }

            return transactionDtos;
        }
        public async Task<TransactionDto> GetTransactionDetailsAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found.");

            var transactionDto = _mapper.Map<TransactionDto>(transaction);
            transactionDto.IconUrl = GetTransactionIcon(transaction.Type);

            return transactionDto;
        }
        public async Task<UserDto> GetUserWithBalanceAsync(Guid userId)
        {
            var user = await _transactionRepository.GetUserWithBalanceAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }
        public string GetTransactionIcon(string type)
        {
            return _transactionRepository.GetIconForTransactionType(type);
        }

    }
}
