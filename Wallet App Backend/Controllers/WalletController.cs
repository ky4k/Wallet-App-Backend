using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Wallet_App_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWallet _walletService;

        public WalletController(IWallet walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<UserDto>> GetUserWithBalanceAsync(Guid userId)
        {
            try
            {
                var user = await _walletService.GetUserWithBalanceAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("transactions/{userId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetLatestTransactionsAsync(Guid userId, int count=10)
        {
            try
            {
                var transactions = await _walletService.GetLatestTransactionsAsync(userId, count);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("daily-points/{userId}")]
        public async Task<ActionResult<string>> GetDailyPointsAsync(Guid userId)
        {
            try
            {
                var points = await _walletService.CalculateDailyPointsAsync(userId, DateTime.UtcNow);
                return Ok(points); 
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message, InnerException = ex.InnerException?.Message });
            }
        }
        [HttpGet("transaction-icon/{type}")]
        public ActionResult<string> GetTransactionIcon(string type)
        {
            try
            {
                var icon = _walletService.GetTransactionIcon(type); 
                return Ok(icon);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching icon for type {type}: {ex.Message}");
            }
        }
    }
}
