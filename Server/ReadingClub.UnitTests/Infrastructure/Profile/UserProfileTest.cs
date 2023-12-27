using AutoMapper;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.Infrastructure.Profile;

namespace ReadingClub.UnitTests.Infrastructure.Profile
{
    public class UserProfileTest
    {
        private readonly IMapper _mapper;
        public UserProfileTest() 
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new UserProfile()));
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public void UserProfile_ShouldMap_User_To_UserDto()
        {
            // Arrange
            var user = new User() 
            {
                Id = 1,
                UserName = "Test",
                Email = "test@test.com",
                Password = "password",
                Salt = "someRandomString",
            };

            // Act
            var userDto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.IsType<UserDto>(userDto);
            Assert.NotNull(userDto);
            Assert.Equal(userDto.UserName, user.UserName);
            Assert.Equal(userDto.Email, user.Email);
        }

        [Fact]  
        public void UserProfile_ShouldMap_UserLoginDto_To_User()
        {
            // Arrange
            var userLoginDto = new UserLoginDto()
            {
                Email = "test@test.com",
                Password = "password"
            };

            // Act
            var user = _mapper.Map<User>(userLoginDto);

            // Assert
            Assert.IsType<User>(user);
            Assert.NotNull(user);
            Assert.Equal(user.Email, userLoginDto.Email);
            Assert.Equal(user.Password, userLoginDto.Password);
        }

        [Fact]
        public void UserProfile_ShouldMap_CreateUserDto_To_User()
        {
            // Arrange
            var createUserDto = new CreateUserDto()
            {
                UserName = "Test",
                Email = "test@test.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            // Act
            var user = _mapper.Map<User>(createUserDto);

            // Assert
            Assert.IsType<User>(user);
            Assert.NotNull(user);
            Assert.Equal(user.UserName, createUserDto.UserName);
            Assert.Equal(user.Email, createUserDto.Email);
            Assert.Equal(user.Password, createUserDto.Password);
        }

        [Fact]
        public void UserProfile_ShouldMap_UpdateUserDto_To_User()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto()
            {
                UserName = "Test",
                Email = "newTest@test.com",
                Password = "password",
                ConfirmPassword = "password",
                OldEmail = "oldTest@test.com",
                IsEditPassword = true
            };

            // Act
            var user = _mapper.Map<User>(updateUserDto);

            // Assert
            Assert.IsType<User>(user);
            Assert.NotNull(user);
            Assert.Equal(user.UserName, updateUserDto.UserName);
            Assert.Equal(user.Email, updateUserDto.Email);
            Assert.Equal(user.Password, updateUserDto.Password);
        }
    }
}
