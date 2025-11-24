using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Service.Implementation
{
    internal class WalletMoneyTransfersService : IWalletMoneyTransfersService
    {
        private readonly IMapper _mapper;
        private readonly IWalletMoneyTransfersRepo walletMoneyTransfersRepo;

        public WalletMoneyTransfersService(IMapper mapper, IWalletMoneyTransfersRepo walletMoneyTransfersRepo)
        {
            _mapper = mapper;
            this.walletMoneyTransfersRepo = walletMoneyTransfersRepo;
        }
        public async Task <TransferWalletResponse> AddAsync(AddWalletTranferMoneyDTO addWalletTranferMoneyDTO)
        {
            var transferEntity = _mapper.Map<WalletMoneyTransfer>(addWalletTranferMoneyDTO);
            try
            {

                await walletMoneyTransfersRepo.AddAsync(transferEntity);
                await walletMoneyTransfersRepo.SaveChangesAsync();
                return new TransferWalletResponse(true, "WalletMoneyTransfer added successfully");  
            }
            catch (Exception ex)
            {

                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new TransferWalletResponse(false, $"Add WalletMoneyTransfer failed: {innerMessage}");


            }
        }

    }
}
