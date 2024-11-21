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
    }
}
