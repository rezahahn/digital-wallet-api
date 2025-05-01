using DigitalWallet.Application.DTOs.Messaging;
using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Exceptions;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Core.Interfaces.Messaging;
using DigitalWallet.Infrastructure.Common.Helpers;
using Microsoft.AspNetCore.Http;

namespace DigitalWallet.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IEventBus _eventBus;

        public UserService(
            IUserRepository userRepository,
            IPasswordHelper passwordHelper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IAuditLogRepository auditLogRepository,
            IEventBus eventBus)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _auditLogRepository = auditLogRepository;
            _eventBus = eventBus;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.ToUserDto();
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user?.ToUserDto();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (await _userRepository.EmailExistsAsync(createUserDto.Email))
                    throw new EmailAlreadyExistsException(createUserDto.Email);

                if (await _userRepository.PhoneNumberExistsAsync(createUserDto.PhoneNumber))
                    throw new PhoneNumberAlreadyExistsException(createUserDto.PhoneNumber);

                var salt = _passwordHelper.GenerateSalt();
                var passwordHash = _passwordHelper.HashPassword(createUserDto.Password, salt);

                var user = new User
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    PhoneNumber = createUserDto.PhoneNumber,
                    FullName = createUserDto.FullName,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    KYCVerified = false,
                    RegistrationDate = DateTime.UtcNow.ToLocalTime(),
                    IsActive = true
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return user.ToUserDto();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _userRepository.PhoneNumberExistsAsync(phoneNumber);
        }

        public async Task<bool> VerifyKycAsync(VerifyKycDto verifyKycDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var user = await _userRepository.GetByIdAsync(verifyKycDto.UserId) ?? throw new UserNotFoundException(verifyKycDto.UserId);

                user.KYCVerified = true;
                await _userRepository.UpdateAsync(user);

                _eventBus.Publish("create_wallet_queue", new CreateWalletMessage
                {
                    UserId = user.UserId
                });

                var auditLog = new AuditLog
                {
                    UserId = user.UserId,
                    ActionType = "KYC_VERIFICATION",
                    ActionDetails = $"Document: {verifyKycDto.DocumentType} - {verifyKycDto.DocumentNumber}",
                    IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
                };

                await _auditLogRepository.AddAsync(auditLog);
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
    }
}
