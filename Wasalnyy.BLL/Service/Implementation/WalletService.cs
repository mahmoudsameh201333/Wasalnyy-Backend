
using AutoMapper;
using System;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Wallet;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;
namespace Wasalnyy.BLL.Service.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepo _walletRepo;
        private readonly IWalletTransactionLogsRepo _transactionRepo;
        private readonly WasalnyyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITripRepo tripRepo;
        private readonly IWalletMoneyTransfersService WalletMoneyTransfersService;
        private readonly RiderService riderService;
        private readonly DriverService driverService;
        private readonly IWalletTransactionService walletTransactionService;
        public WalletService(
            IWalletRepo walletRepo,
            IWalletTransactionLogsRepo transactionRepo,
            WasalnyyDbContext context, IMapper mapper,ITripRepo tripRepo, IWalletTransactionService walletTransactionService, IWalletMoneyTransfersService WalletMoneyTransfersService
            , RiderService riderService ,DriverService driverService)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _context = context;
            _mapper = mapper;
            this.tripRepo = tripRepo;
            this.walletTransactionService = walletTransactionService;
            this.WalletMoneyTransfersService = WalletMoneyTransfersService;
            this.riderService= riderService;
            this.driverService= driverService;
        }

        // ============================================================
        //   GET WALLET
        // ============================================================

        public async Task<Wallet?> GetWalletOfUserIdAsync(string userId)
        {
            return await _walletRepo.GetWalletOfUserIdAsync(userId);
        }


        // ============================================================
        //   ADD MONEY TO WALLET
        // ============================================================

        public async Task<IncreaseWalletBalanceResponse> IncreaseWalletAsync(IncreaseWalletDTO increaseWalletDTO)
        {


            if (increaseWalletDTO.Amount <= 0)
                return new IncreaseWalletBalanceResponse(false ,"Amount of money cant be negative or zero");


            try
            {
                var wallet = await _walletRepo.GetWalletOfUserIdAsync(increaseWalletDTO.UserId);
                if (wallet == null)
                    return new IncreaseWalletBalanceResponse(false, "This User doesnt have Wallet Call Dev to make sure Rider or driver User have wallet created");
                wallet.Balance += increaseWalletDTO.Amount;
                wallet.ModifiedAt = increaseWalletDTO.DateTime;
                await _walletRepo.UpdateWalletAsync(wallet);
                await _walletRepo.SaveChangesAsync();


                var res =  await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
                {
                    WalletId = wallet.Id,
                    Amount = increaseWalletDTO.Amount,
                    TransactionType = DAL.Enum.WalletTransactionType.Credit,
                    Description = $"user charge his wallet by {increaseWalletDTO.Amount}",
                    CreatedAt = increaseWalletDTO.DateTime

                });

                if(!res.isSuccess)
                    return new IncreaseWalletBalanceResponse(false, $"balance value is increased but An error occurred while creating wallet transaction log: {res.Message}");


                return new IncreaseWalletBalanceResponse(true, "Wallet balance increased successfully");
                }

                

            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new IncreaseWalletBalanceResponse(false, $"An error occurred while processing payment: {innerMessage}");
            }

         

           
        }



        public async Task<TransferWalletResponse> HandleTransferWalletMoneyFromRiderToDriver(TransferMoneyBetweenUsersDTO transferDto)
        {
            // VALIDATE EVERYTHING FIRST (before starting transaction)
            var rider = await riderService.GetByIdAsync(transferDto.RiderId);
            if (rider == null)
                return new TransferWalletResponse(false, "Rider not found");

            var driver = await driverService.GetByIdAsync(transferDto.DriverId);
            if (driver == null)
                return new TransferWalletResponse(false, "Driver not found");

            var trip = await tripRepo.GetByIdAsync(transferDto.TripId);
            if (trip == null)
                return new TransferWalletResponse(false, "Trip not found");

            var riderWallet = await _walletRepo.GetWalletOfUserIdAsync(transferDto.RiderId);
            if (riderWallet == null)
                return new TransferWalletResponse(false, "Rider Wallet not found");

            var driverWallet = await _walletRepo.GetWalletOfUserIdAsync(transferDto.DriverId);
            if (driverWallet == null)
                return new TransferWalletResponse(false, "Driver Wallet not found");

            if (riderWallet.Balance < transferDto.Amount)
                return new TransferWalletResponse(false, "Insufficient balance");

            // NOW start transaction - all validations passed
            using var transaction = await _walletRepo.BeginTransactionAsync();

            try
            {
                // Update balances
                riderWallet.Balance -= transferDto.Amount;
                driverWallet.Balance += transferDto.Amount;
                riderWallet.ModifiedAt = transferDto.CreatedAt;
                driverWallet.ModifiedAt = transferDto.CreatedAt;

                await _walletRepo.UpdateWalletWithoutSaving(riderWallet);
                await _walletRepo.UpdateWalletWithoutSaving(driverWallet);

                // Create transaction logs...
                var res = await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
                {
                    WalletId = riderWallet.Id,
                    Amount = transferDto.Amount,
                    TransactionType = DAL.Enum.WalletTransactionType.Debit,
                    Description = $"Rider Pay {transferDto.Amount} for trip ID {trip.Id} to Driver Id{driver.Id}",
                    CreatedAt = transferDto.CreatedAt
                });

                if (!res.isSuccess)
                {
                    await transaction.RollbackAsync();
                    return new TransferWalletResponse(false, $"Failed to create rider transaction log: {res.Message}");
                }

                var res2 = await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
                {
                    WalletId = driverWallet.Id,
                    Amount = transferDto.Amount,
                    TransactionType = DAL.Enum.WalletTransactionType.Credit,
                    Description = $"Driver receive {transferDto.Amount} for trip ID {trip.Id} from Rider Id{rider.RiderId}",
                    CreatedAt = transferDto.CreatedAt
                });

                if (!res2.isSuccess)
                {
                    await transaction.RollbackAsync();
                    return new TransferWalletResponse(false, $"Failed to create driver transaction log: {res2.Message}");
                }

                var res3 = await WalletMoneyTransfersService.AddAsync(new AddWalletTranferMoneyDTO
                {
                    CreatedAt = transferDto.CreatedAt,
                    Amount = transferDto.Amount,
                    SenderWalletId = riderWallet.Id,
                    ReceiverWalletId = driverWallet.Id,
                    TripId = transferDto.TripId,
                });

                if (!res3.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return new TransferWalletResponse(false, $"Failed to create transfer record: {res3.Message}");
                }

                // Save all changes
                await _walletRepo.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return new TransferWalletResponse(true, "Transfer completed successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return new TransferWalletResponse(false, $"Transaction failed: {innerMessage}");
            }
        }
        //public async Task<TransferWalletResponse> HandleTransferWalletMoneyFromRiderToDriver(TransferMoneyBetweenUsersDTO transferDto)
        //{


        //    //1-check if this rider valid or not  
        //    var rider = await riderService.GetByIdAsync(transferDto.RiderId);
        //    if (rider == null)
        //        return new TransferWalletResponse(false, "Rider not found ");
        //    //2-check if this driver valid or not
        //    var driver = await driverService.GetByIdAsync(transferDto.DriverId);
        //    if (driver == null)
        //        return new TransferWalletResponse(false, "Driver not found ");

        //    using var transaction =  await _walletRepo.BeginTransactionAsync();

        //    try
        //    {

        //        // 3- Get wallets
        //        var riderWallet = await _walletRepo.GetWalletOfUserIdAsync(transferDto.RiderId);
        //        var driverWallet = await _walletRepo.GetWalletOfUserIdAsync(transferDto.DriverId);

        //        if (riderWallet == null )
        //            return new TransferWalletResponse(false, "Rider Wallet not found ");

        //        if ( driverWallet == null)
        //            return new TransferWalletResponse(false, "Driver Wallet not found ");

        //        // 4- Check balance
        //        if (riderWallet.Balance < transferDto.Amount)
        //            return new TransferWalletResponse(false, "Insufficient balance");

        //        //3-check if the trip id is exist or not 


        //      var trip=  await tripRepo.GetByIdAsync(transferDto.TripId);
        //        if (trip == null)
        //            return new TransferWalletResponse(false, "Transfering failed Trip not found ");

        //        // 5- update balances and update Lasttimeupdatedate date 
        //        riderWallet.Balance -= transferDto.Amount;
        //        driverWallet.Balance += transferDto.Amount;


        //        riderWallet.ModifiedAt = transferDto.CreatedAt;
        //        driverWallet.ModifiedAt = transferDto.CreatedAt;

        //        await   _walletRepo.UpdateWalletWithoutSaving(riderWallet);
        //        await   _walletRepo.UpdateWalletWithoutSaving(driverWallet);


        //        //6- create  WALLET transaction log for both wallets
        //        //Rider wallet TransactionLog
        //        var res = await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
        //        {
        //            WalletId = riderWallet.Id,
        //            Amount = transferDto.Amount,
        //            TransactionType = DAL.Enum.WalletTransactionType.Debit,
        //            Description = $"Rider Pay {transferDto.Amount} for trip ID {trip.Id} to Driver Id{driver.Id}",
        //            CreatedAt = transferDto.CreatedAt

        //        });

        //        if (!res.isSuccess)
        //        {
        //            await transaction.RollbackAsync();

        //            return new TransferWalletResponse(false, $"Transfering failed An error occurred while creating wallet transaction log: {res.Message}");
        //        }
        //        //Driver wallet transaction log
        //        var res2 = await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
        //        {
        //            WalletId = driverWallet.Id,
        //            Amount = transferDto.Amount,
        //            TransactionType = DAL.Enum.WalletTransactionType.Credit,
        //            Description = $"Driver recieve {transferDto.Amount} for trip ID {trip.Id} from Rider Id{rider.RiderId}",
        //            CreatedAt = transferDto.CreatedAt

        //        });

        //        if (!res2.isSuccess)
        //        {
        //            await transaction.RollbackAsync();

        //            return new TransferWalletResponse(false, $"Transfering failed An error occurred while creating wallet transaction log: {res2.Message}");
        //        }

        //        //7-insert this transfer transaction in the transferTransaction table

        //      var res3=  await WalletMoneyTransfersService.AddAsync(new AddWalletTranferMoneyDTO
        //        {

        //            CreatedAt = transferDto.CreatedAt,
        //            Amount = transferDto.Amount,
        //            SenderWalletId = riderWallet.Id,
        //            ReceiverWalletId = driverWallet.Id,
        //            TripId = transferDto.TripId,

        //        });

        //        if (!res3.IsSuccess)
        //        {
        //            await transaction.RollbackAsync();

        //            return new TransferWalletResponse(false, $"Transfering failed An error occurred while inserting Transfer Transaction  {res3.Message}");
        //        }

        //        // 8- save all changes
        //        await _walletRepo.SaveChangesAsync();

        //        // 9- commit
        //        await transaction.CommitAsync();

        //        return new TransferWalletResponse(true, "Transfer completed successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        // rollback
        //        await transaction.RollbackAsync();

        //        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        return new TransferWalletResponse(false, $"Transaction failed: {innerMessage}");
        //    }




        //}

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletDTO createWalletDTO)
        {
            // Map DTO to entity
            var wallet = _mapper.Map<Wallet>(createWalletDTO);

            // Save to repository
            try
            {
                await _walletRepo.CreateAsync(wallet);
                await _walletRepo.SaveChangesAsync();
                return new CreateWalletResponse(true, "Wallet Created succesfully ");
            }
            catch  (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new CreateWalletResponse(false, $"Creating Wallet Failed: {innerMessage}");

            }
        }


       

        public async Task<WithDrawFromWalletResponse> WithdrawFromWalletAsync(WithdrawFromWalletDto withdrawFromWalletDto)

        {
            //var TransactionLog = _mapper.Map<CreateWalletTransactionDTO>(withdrawFromWalletDto);

            if (withdrawFromWalletDto.Amount <= 0)
                return new WithDrawFromWalletResponse(false,"Amount Can't be negative or zero");




            var wallet = await _walletRepo.GetWalletOfUserIdAsync(withdrawFromWalletDto.UserId);
            if (wallet == null)
                return new WithDrawFromWalletResponse(false, "This User doesnt hvae a wallet");



            if (wallet.Balance < withdrawFromWalletDto.Amount)
               return new WithDrawFromWalletResponse(false, "Balance is not enough");


            wallet.Balance -= withdrawFromWalletDto.Amount;
            wallet.ModifiedAt =withdrawFromWalletDto.CreatedAt;

            await _walletRepo.UpdateWalletAsync(wallet);

            await _walletRepo.SaveChangesAsync();

            var res = await walletTransactionService.CreateAsync(new CreateWalletTransactionLogDTO
            {
                WalletId = wallet.Id,
                Amount = withdrawFromWalletDto.Amount,
                TransactionType = DAL.Enum.WalletTransactionType.Debit,
                Description = $"user withdraw from his wallet by {withdrawFromWalletDto.Amount}",
                CreatedAt = withdrawFromWalletDto.CreatedAt

            });

            if (!res.isSuccess)
                return new WithDrawFromWalletResponse(false, $"Withdraw done but An error occurred while creating wallet transaction log: {res.Message}");


           return new WithDrawFromWalletResponse(true, "Withdraw from wallet done successfully");

        }


        // ============================================================
        //   TRANSFER (Rider -> Driver)
        // ============================================================

        //public async Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string? tripId = null)
        //{
        //    if (amount <= 0)
        //        return false;

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    var senderWallet = await _walletRepo.GetByUserIdAsync(fromUserId);
        //    var receiverWallet = await _walletRepo.GetByUserIdAsync(toUserId);

        //    if (senderWallet == null || receiverWallet == null)
        //        return false;

        //    if (senderWallet.Balance < amount)
        //        return false;

        //    // Sender
        //    senderWallet.Balance -= amount;
        //    senderWallet.ModifiedAt = DateTime.UtcNow;
        //    await _walletRepo.UpdateAsync(senderWallet);

        //    var senderLog = new WalletTransaction
        //    {
        //        WalletId = senderWallet.Id,
        //        Amount = amount,
        //        TransactionType = WalletTransactionType.Debit,
        //        Description = $"Trip Payment {tripId}",
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    await _transactionRepo.CreateAsync(senderLog);


        //    // Receiver
        //    receiverWallet.Balance += amount;
        //    receiverWallet.ModifiedAt = DateTime.UtcNow;
        //    await _walletRepo.UpdateAsync(receiverWallet);

        //    var receiverLog = new WalletTransaction
        //    {
        //        WalletId = receiverWallet.Id,
        //        Amount = amount,
        //        TransactionType = WalletTransactionType.Credit,
        //        Description = $"Trip Payment Received {tripId}",
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    await _transactionRepo.CreateAsync(receiverLog);

        //    await _walletRepo.SaveChangesAsync();
        //    await _transactionRepo.SaveChangesAsync();

        //    await transaction.CommitAsync();
        //    return true;
        //}




        public async Task<bool> CheckUserBalanceAsync(string userId, decimal amount)
        {
            var wallet = await _walletRepo.GetWalletOfUserIdAsync(userId);

            return wallet != null && wallet.Balance >= amount;
            
        }

        public async Task<decimal> GetWalletBalance(string userId)
        {
            var wallet = await _walletRepo.GetWalletOfUserIdAsync(userId);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found for the specified user ID.");
            return wallet.Balance;
        }
    }
}

