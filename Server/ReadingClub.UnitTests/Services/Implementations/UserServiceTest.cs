using AutoMapper;
using Moq;
using ReadingClub.Infrastructure.Profile;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Implementations;
using ReadingClub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

        #region Create

        #endregion

        #region Delete

        #endregion

        #region Get(string userEmail)

        #endregion

        #region Get(UserLoginDto userLogin)

        #endregion

        #region Update

        #endregion
    }
}
