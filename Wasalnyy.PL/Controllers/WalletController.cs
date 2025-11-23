using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.DTO;
using Wasalnyy.BLL.DTO.Wallet;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IMapper mapper)
        {
            _walletService = walletService;
            _mapper = mapper;
        }

        // GET: api/wallet/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWallet(string userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            if (wallet == null)
                return NotFound(new { message = "Wallet not found" });

            return Ok(_mapper.Map<WalletDto>(wallet));
        }

        //// POST: api/wallet/user/{userId}/add
        //[HttpPost("user/{userId}/add")]
        //public async Task<IActionResult> AddToWallet(string userId, [FromBody] decimal amount, [FromQuery] string? reference = null)
        //{
        //    var success = await _walletService.IncreaseWalletAsync(userId, amount);
        //    if (!success)
        //        return BadRequest(new { message = "Failed to add money. Check amount or user." });

        //    var wallet = await _walletService.GetWalletByUserIdAsync(userId);
        //    return Ok(_mapper.Map<WalletDto>(wallet));
        //}

        // POST: api/wallet/user/{userId}/withdraw
        [HttpPost("user/{userId}/withdraw")]
        public async Task<IActionResult> WithdrawFromWallet(string userId, [FromBody] decimal amount, [FromQuery] string? reference = null)
        {
            var success = await _walletService.WithdrawFromWalletAsync(userId, amount, reference);
            if (!success)
                return BadRequest(new { message = "Failed to withdraw money. Check balance or user." });

            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            return Ok(_mapper.Map<WalletDto>(wallet));
        }

        // POST: api/wallet/transfer
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromQuery] string fromUserId, [FromQuery] string toUserId, [FromQuery] decimal amount, [FromQuery] string? tripId = null)
        {
            var success = await _walletService.TransferAsync(fromUserId, toUserId, amount, tripId);
            if (!success)
                return BadRequest(new { message = "Transfer failed. Check balances or users." });

            var senderWallet = await _walletService.GetWalletByUserIdAsync(fromUserId);
            var receiverWallet = await _walletService.GetWalletByUserIdAsync(toUserId);

            return Ok(new
            {
                Sender = _mapper.Map<WalletDto>(senderWallet),
                Receiver = _mapper.Map<WalletDto>(receiverWallet)
            });
        }

        // GET: api/wallet/user/{userId}/transactions
        [HttpGet("user/{userId}/transactions")]
        public async Task<IActionResult> GetTransactions(string userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            if (wallet == null)
                return NotFound(new { message = "Wallet not found" });

            var transactionsDto = _mapper.Map<IEnumerable<WalletTransactionDto>>(wallet.Transactions.OrderByDescending(t => t.CreatedAt));
            return Ok(transactionsDto);
        }
    }
}
