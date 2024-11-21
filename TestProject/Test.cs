using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using DAL.Models;
using BLL.DTOs;
using DAL.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class WalletServiceTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private Mock<IMapper> _mockMapper;
        private IWallet _walletService;

        [SetUp]
        public void Setup()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _walletService = new WalletService(_mockTransactionRepository.Object, _mockMapper.Object);

            _mockTransactionRepository
           .Setup(repo => repo.GetIconForTransactionType("Payment"))
           .Returns("/icons/payment-icon.png");

            _mockTransactionRepository
                .Setup(repo => repo.GetIconForTransactionType("Credit"))
                .Returns("/icons/credit-icon.png");

            _mockTransactionRepository
                .Setup(repo => repo.GetIconForTransactionType(It.IsNotIn("Payment", "Credit")))
                .Returns("/icons/default-icon.png");
        }

        [Test]
        public async Task CalculateDailyPointsAsync_ShouldReturnCorrectPoints_ForFirstDayOfSeason()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                LastPointsDate = DateTime.MinValue,
                Points = 0
            };

            _mockTransactionRepository.Setup(repo => repo.GetUserWithBalanceAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _walletService.CalculateDailyPointsAsync(userId, new DateTime(2024, 3, 1)); // First day of spring

            // Assert
            result.Should().Be("2");
            _mockTransactionRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task CalculateDailyPointsAsync_ShouldReturnExistingPoints_IfAlreadyCalculated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var today = DateTime.Today;
            var user = new User
            {
                Id = userId,
                LastPointsDate = today,
                Points = 10
            };

            _mockTransactionRepository.Setup(repo => repo.GetUserWithBalanceAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _walletService.CalculateDailyPointsAsync(userId, today);

            // Assert
            result.Should().Be("10");
            _mockTransactionRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task GetUserWithBalanceAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "John Doe",
                CardBalance = 1000,
                CardLimit = 1500,
                Points = 10
            };

            var userDto = new UserDto
            {
                Id = userId,
                Name = "John Doe",
                CardBalance = 1000,
                CardLimit = 1500
            };

            _mockTransactionRepository.Setup(repo => repo.GetUserWithBalanceAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _walletService.GetUserWithBalanceAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("John Doe");
            result.CardBalance.Should().Be(1000);
        }

        [Test]
        public async Task GetLatestTransactionsAsync_ShouldReturnTransactions_WhenTransactionsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), UserId = userId, Type = "Payment", Amount = 100, Name = "IKEA", User = new User() },
            new Transaction { Id = Guid.NewGuid(), UserId = userId, Type = "Credit", Amount = 50, Name = "Target", User = new User() }
        };

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Amount = t.Amount,
                Name = t.Name
            }).ToList();

            _mockTransactionRepository.Setup(repo => repo.GetTransactionsByUserIdAsync(userId, 10)).ReturnsAsync(transactions);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TransactionDto>>(transactions)).Returns(transactionDtos);

            // Act
            var result = await _walletService.GetLatestTransactionsAsync(userId, 10);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.Type == "Payment" || t.Type == "Credit");
        }

        [Test]
        public void GetTransactionIcon_ShouldReturnCorrectIcon_ForPaymentType()
        {
            // Act
            var result = _walletService.GetTransactionIcon("Payment");

            // Assert
            result.Should().Be("/icons/payment-icon.png");
        }

        [Test]
        public void GetTransactionIcon_ShouldReturnCorrectIcon_ForCreditType()
        {
            // Act
            var result = _walletService.GetTransactionIcon("Credit");

            // Assert
            result.Should().Be("/icons/credit-icon.png");
        }

        [Test]
        public void GetTransactionIcon_ShouldReturnDefaultIcon_ForUnknownType()
        {
            // Act
            var result = _walletService.GetTransactionIcon("Unknown");

            // Assert
            result.Should().Be("/icons/default-icon.png");
        }
    }
}

