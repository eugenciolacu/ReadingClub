using Microsoft.Extensions.Configuration;
using Moq;
using ReadingClub.Controllers;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.UnitTests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _mockUserService= new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _userController = new UserController(_mockUserService.Object, _mockConfiguration.Object);
        }

        #region Create

        #endregion

        #region Login

        #endregion

        #region IsTokenValidAsync

        #endregion

        #region Get

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion

        #region GetLoggedUser

        #endregion
    }
}
