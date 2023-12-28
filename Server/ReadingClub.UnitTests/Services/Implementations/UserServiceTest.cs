using AutoMapper;
using Moq;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Helpers;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.Infrastructure.Profile;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Implementations;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.UnitTests.Services.Implementations
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IBookRepository> _mockBookRepository;

        private readonly IMapper _mapper;

        private readonly IUserService _userService;

        public UserServiceTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new UserProfile()));
            _mapper = new Mapper(configuration);

            _mockUserRepository = new Mock<IUserRepository>();
            _mockBookRepository = new Mock<IBookRepository>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockBookRepository.Object,
                _mapper
            );
        }

        #region CheckIfUserExists 
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CheckIfUserExists_WithValidInput_ReturnsInt(int checkIfExistsPossibleResults)
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.CheckIfExists(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(checkIfExistsPossibleResults);

            // Act
            var result = _userService.CheckIfUserExists(new ReadingClub.Infrastructure.DTO.User.CreateUserDto() { });
        
            // Assert
            Assert.IsType<Task<int>>(result);
            Assert.InRange(result.Result, 0, 3);
        }
        #endregion

        #region Create
        [Fact]
        public void Create_WithValidInput_ReturnsUserDto()
        {
            // Arrange
            var createUserDto = new CreateUserDto()
            {
                UserName = "Test",
                Email = "test@test.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            _mockUserRepository.Setup(repo => repo.Create(It.IsAny<User>()))
                .ReturnsAsync(new User()
                {
                    Id = 123,
                    UserName = "Test",
                    Email = "test@test.com",
                    Password = "hashedPassword",
                    Salt = "someNotNullSalt"
                });

            // Act
            var result = _userService.Create(createUserDto);

            // Assert
            Assert.IsType<Task<UserDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(createUserDto.UserName, result.Result.UserName);
            Assert.Equal(createUserDto.Email, result.Result.Email);
        }

        #endregion

        #region Delete
        [Fact]
        public void Delete_WithValidInput_ReturnsVoid()
        {
            // Arrange
            var anonymousUserId = 1; // anonymous user have Id = 1
            var anonymousUser = "anonymous";

            _mockUserRepository.Setup(repo => repo.GetUserIdByEmail(anonymousUser))
                .ReturnsAsync(anonymousUserId);

            //_mockBookRepository.Setup(repo => repo.ClearBooksForUserBeforeDelete(It.IsAny<string>(), anonymousUserId));
            //_mockUserRepository.Setup(repo => repo.Delete(It.IsAny<string>()));

            // Act
            var result = _userService.Delete("someEmail@test.com");

            // Assert           
            _mockBookRepository.Verify(repo => repo.ClearBooksForUserBeforeDelete(It.IsAny<string>(), anonymousUserId), Times.Once);
            
            _mockUserRepository.Verify(repo => repo.Delete(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Get(string userEmail)
        [Fact]
        public void Get_ByEmail_WithValidInput_ReturnsUserDto()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.Get(It.IsAny<string>()))
                .ReturnsAsync(new User()
                {
                    Id = 123,
                    UserName = "Test",
                    Email = "test@test.com",
                    Password = "hashedPassword",
                    Salt = "someNotNullSalt"
                });
            // Act
            var result = _userService.Get("test@test.com");

            // Assert
            Assert.IsType<Task<UserDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal("Test", result.Result.UserName);
            Assert.Equal("test@test.com", result.Result.Email);
        }
        #endregion

        #region Get(UserLoginDto userLogin)
        [Fact]
        public void Get_ByUserLoginDto_WithValidInput_ReturnsUserDto()
        {
            // Arrange
            var loggedUser = new User()
            {
                Id = 123,
                UserName = "Test",
                Email = "test@test.com",
                Password = "Password",
                Salt = "/someNotNullSalt"
            };

            _mockUserRepository.Setup(repo => repo.Get(It.IsAny<string>()))
                .ReturnsAsync(loggedUser)
                .Callback(() => { loggedUser.Password = PasswordHasher.HashPassword(loggedUser.Password, loggedUser.Salt); });

            // Act
            var result = _userService.Get(new UserLoginDto()
            {
                Email = "test@test.com",
                Password = "Password",
            });

            // Assert
            Assert.IsType<Task<UserDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal("Test", result.Result.UserName);
            Assert.Equal("test@test.com", result.Result.Email);
        }

        [Fact]
        public void Get_ByUserLoginDto_WithInValidInput_WrongPassword_ReturnsNull()
        {
            // Arrange
            var loggedUser = new User()
            {
                Id = 123,
                UserName = "Test",
                Email = "test@test.com",
                Password = "Password",
                Salt = "/someNotNullSalt"
            };

            _mockUserRepository.Setup(repo => repo.Get(It.IsAny<string>()))
                .ReturnsAsync(loggedUser)
                .Callback(() => { loggedUser.Password = PasswordHasher.HashPassword(loggedUser.Password, loggedUser.Salt); });

            // Act
            var result = _userService.Get(new UserLoginDto()
            {
                Email = "test@test.com",
                Password = "WrongPassword",
            });

            // Assert
            Assert.IsType<Task<UserDto>>(result);
            Assert.Null(result.Result);
        }
        #endregion

        #region Update
        [Fact]
        public void Update_WithValidInput_ReturnsUserDto()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto() {
                UserName = "UpdatedUserName",
                Email = "updatedEmail@test.com",
                Password = "Password",
                ConfirmPassword = "Password",
                OldEmail = "test@test.com",
                IsEditPassword = true
            };

            var user = new User()
            {
                Id = 0,
                UserName = "UpdatedUserName",
                Email = "updatedEmail@test.com",
                Password = "Password",
                Salt = "/someNotNullSalt"
            };

            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(user)
                .Callback(() => { user.Password = PasswordHasher.HashPassword(user.Password, user.Salt); });

            // Act
            var result = _userService.Update(updateUserDto);

            // Assert
            Assert.IsType<Task<UserDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(updateUserDto.UserName, result.Result.UserName);
            Assert.Equal(updateUserDto.Email, result.Result.Email);
        }
        #endregion
    }
}
