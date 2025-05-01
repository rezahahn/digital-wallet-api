using AutoMapper;
using DigitalWallet.Application.DTOs.Messaging;
using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.DTOs.Wallet.Request;
using DigitalWallet.Application.DTOs.Wallet.Response;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Exceptions;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Core.Interfaces.Messaging;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DigitalWallet.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IEventBus _eventBus;

        public WalletService(
            IWalletRepository walletRepository,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WalletService> logger,
            IDistributedCache cache,
            IEventBus eventBus)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _eventBus = eventBus;
        }

        public async Task<WalletDto?> GetWalletByUserIdAsync(int userId)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new UserNotFoundException(userId);
            }

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            return wallet?.ToWalletDto();
        }

        public async Task<decimal> GetWalletBalanceAsync(int userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                throw new WalletNotFoundException($"Wallet for user {userId} not found");
            }

            return wallet.Balance;
        }

        public async Task<TopUpResponse> TopUpAsync(TopUpRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
                if (wallet == null)
                    throw new WalletNotFoundException(request.UserId);

                wallet.Balance += request.Amount;
                wallet.LastUpdated = DateTime.UtcNow.ToLocalTime();

                var transaction = new Transaction
                {
                    WalletId = wallet.WalletId,
                    Amount = request.Amount,
                    TransactionType = "TopUp",
                    Description = $"Top up via {request.PaymentMethod}",
                    Status = "Completed",
                    ReferenceNumber = Guid.NewGuid().ToString(),
                    TransactionDate = DateTime.UtcNow.ToLocalTime()
                };

                await _transactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _eventBus.Publish(queueName: "payment_notifications_status", message: new PaymentNotificationMessage
                {
                    TransactionId = transaction.TransactionId,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = "IDR",
                    PaymentMethod = request.PaymentMethod,
                    ReferenceNumber = transaction.ReferenceNumber,
                    Status = "Success",
                    Timestamp = DateTime.UtcNow.ToLocalTime(),
                });

                await _unitOfWork.CommitAsync();

                return new TopUpResponse
                {
                    Success = true,
                    TransactionId = transaction.TransactionId.ToString(),
                    ReferenceNumber = transaction.ReferenceNumber,
                    NewBalance = wallet.Balance,
                    TransactionDate = transaction.TransactionDate
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _eventBus.Publish(queueName: "payment_notifications_status", message: new PaymentNotificationMessage
                {
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = "IDR",
                    PaymentMethod = request.PaymentMethod,
                    Timestamp = DateTime.UtcNow.ToLocalTime(),
                    Status = "Failed",
                    FailureReason = ex.Message
                });
                throw;
            }
        }

        public async Task<TransferResponse> TransferAsync(TransferRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var fromWallet = await _walletRepository.GetByUserIdAsync(request.FromUserId);
                if (fromWallet == null)
                    throw new WalletNotFoundException(request.FromUserId);

                var toWallet = await _walletRepository.GetByUserIdAsync(request.ToUserId);
                if (toWallet == null)
                    throw new WalletNotFoundException(request.ToUserId);

                if (fromWallet.Balance < request.Amount)
                    throw new InsufficientBalanceException();

                fromWallet.Balance -= request.Amount;
                toWallet.Balance += request.Amount;

                var transactionFrom = new Transaction
                {
                    WalletId = fromWallet.WalletId,
                    Amount = request.Amount,
                    TransactionType = "TransferOut",
                    Description = request.Description,
                    Status = "Completed",
                    ReferenceNumber = Guid.NewGuid().ToString(),
                    TransactionDate = DateTime.UtcNow.ToLocalTime()
                };

                await _transactionRepository.AddAsync(transactionFrom);
                await _unitOfWork.SaveChangesAsync();

                var transactionTo = new Transaction
                {
                    WalletId = toWallet.WalletId,
                    Amount = request.Amount,
                    TransactionType = "TransferIn",
                    Description = request.Description,
                    Status = "Completed",
                    ReferenceNumber = Guid.NewGuid().ToString(),
                    RelatedTransactionId = transactionFrom.TransactionId,
                    TransactionDate = DateTime.UtcNow.ToLocalTime()
                };

                await _transactionRepository.AddAsync(transactionTo);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new TransferResponse
                {
                    Success = true,
                    TransactionId = transactionFrom.TransactionId.ToString(),
                    ReferenceNumber = transactionFrom.ReferenceNumber,
                    SenderNewBalance = fromWallet.Balance,
                    ReceiverNewBalance = toWallet.Balance,
                    TransactionDate = transactionFrom.TransactionDate
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CreateWalletForUser(int userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (await _walletRepository.UserHasWalletAsync(userId))
                    return true;

                var wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    Currency = "IDR",
                    LastUpdated = DateTime.UtcNow.ToLocalTime()
                };

                await _walletRepository.AddAsync(wallet);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<WalletTransactionsResponse> GetWalletTransactionsAsync(
        int walletId,
        DateTime? fromDate,
        DateTime? toDate,
        string? transactionType,
        string? status,
        string sortBy,
        bool descending,
        int page,
        int pageSize)
        {
            if (!await _walletRepository.WalletExistsAsync(walletId))
                throw new WalletNotFoundException(walletId);

            var cacheKey = $"wallet:{walletId}:txns:" +
                          $"{fromDate?.ToString("yyyyMMdd")}:{toDate?.ToString("yyyyMMdd")}:" +
                          $"{transactionType}:{status}:{sortBy}:{descending}:{page}:{pageSize}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Retrieved transactions from cache for wallet {WalletId}", walletId);
                return JsonSerializer.Deserialize<WalletTransactionsResponse>(cachedData);
            }

            var (transactions, totalCount) = await _transactionRepository
                .GetTransactionsByWalletIdAsync(
                    walletId,
                    fromDate,
                    toDate,
                    transactionType,
                    status,
                    sortBy,
                    descending,
                    page,
                    pageSize);

            var response = new WalletTransactionsResponse
            {
                Transactions = _mapper.Map<IEnumerable<WalletTransactionDto>>(transactions),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                FilterFromDate = fromDate,
                FilterToDate = toDate,
                FilterType = transactionType,
                FilterStatus = status
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(response),
                cacheOptions);

            return response;
        }
    }
}
