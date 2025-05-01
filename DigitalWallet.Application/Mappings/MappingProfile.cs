using AutoMapper;
using DigitalWallet.Application.DTOs.Transaction;
using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Core.Entities;

namespace DigitalWallet.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Salt, opt => opt.Ignore());

            // Wallet mappings
            CreateMap<Wallet, WalletDto>();

            // Transaction mappings
            CreateMap<Transaction, TransactionDto>();
            CreateMap<Transaction, WalletTransactionDto>();
        }
    }
}
