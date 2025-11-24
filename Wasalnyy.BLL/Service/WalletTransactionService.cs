using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Service
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IMapper _mapper;
        private readonly IWalletTransactionRepo walletTransactionRepo;

        public WalletTransactionService(IMapper mapper, IWalletTransactionRepo walletTransactionRepo)
        {
            _mapper = mapper;
            this.walletTransactionRepo = walletTransactionRepo;
        }
        public async Task<CreateWalletTransactionLogResponse> CreateAsync(CreateWalletTransactionDTO createWalletTransactionDTO)
        {
            var walletTransaction = _mapper.Map<WalletTransactionLogs>(createWalletTransactionDTO);
            try
            {
                await walletTransactionRepo.CreateAsync(walletTransaction);
                await walletTransactionRepo.SaveChangesAsync();
                return new CreateWalletTransactionLogResponse(true, "WalletTransaction Logs added successfully");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new CreateWalletTransactionLogResponse(false, $"Add WalletTransaction Logs saving failed: {innerMessage}");
            }
        }

    }
}
