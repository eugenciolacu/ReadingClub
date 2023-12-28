using AutoMapper;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Helpers;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IBookRepository bookRepository, IMapper mapper) 
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<int> CheckIfUserExists(CreateUserDto createUserDto)
        {
            return await _userRepository.CheckIfExists(createUserDto.UserName, createUserDto.Email);
        }

        public async Task<UserDto> Create(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);

            string salt = PasswordHasher.GenerateSalt();
            string hashedPassword = PasswordHasher.HashPassword(createUserDto.Password, salt);

            user.Password = hashedPassword;
            user.Salt = salt;

            var createdUser = await _userRepository.Create(user);

            var userDto = _mapper.Map<UserDto>(createdUser);

            return userDto;
        }

        public async Task Delete(string userEmail)
        {
            var anonymousUserId = await _userRepository.GetUserIdByEmail("anonymous");

            await _bookRepository.ClearBooksForUserBeforeDelete(userEmail, anonymousUserId);

            await _userRepository.Delete(userEmail);
        }

        public async Task<UserDto> Get(string userEmail)
        {
            var user = await _userRepository.Get(userEmail);

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        public async Task<UserDto> Get(UserLoginDto userLogin)
        {
            var loggedUser = await _userRepository.Get(userLogin.Email);

            if (loggedUser != null)
            {
                string salt = loggedUser.Salt;
                string hashedPassword = PasswordHasher.HashPassword(userLogin.Password, salt);

                if (hashedPassword != loggedUser.Password)
                {
                    return null!;
                }
            }

            var userDto = _mapper.Map<UserDto>(loggedUser);

            return userDto;
        }

        public async Task<UserDto> Update(UpdateUserDto updateUserDto)
        {
            var user = _mapper.Map<User>(updateUserDto);

            if (updateUserDto.IsEditPassword)
            {
                string salt = PasswordHasher.GenerateSalt();
                string hashedPassword = PasswordHasher.HashPassword(updateUserDto.Password, salt);
                user.Password = hashedPassword;
                user.Salt = salt;
            }

            var updatedUser = await _userRepository.Update(user, updateUserDto.OldEmail, updateUserDto.IsEditPassword);

            var userDto = _mapper.Map<UserDto>(updatedUser);

            return userDto;
        }
    }
}
