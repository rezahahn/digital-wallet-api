using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Core.Entities;

namespace DigitalWallet.Application.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                KYCVerified = user.KYCVerified,
                RegistrationDate = user.RegistrationDate
            };
        }
    }
}
