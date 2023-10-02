using ReadingClub.Domain;
using ReadingClub.Infrastructure.DTO.User;

namespace ReadingClub.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> CheckIfUserExists(CreateUserDto createUserDto);
        Task<UserDto> Create(CreateUserDto createUserDto);
        Task Delete(string userEmail);
        //Task<UserDto> Get(int id);
        Task<UserDto> Get(string userEmail);
        Task<UserDto> Get(UserLoginDto userLogin);
        Task<UserDto> Update(UpdateUserDto updateUserDto);
    }
}
