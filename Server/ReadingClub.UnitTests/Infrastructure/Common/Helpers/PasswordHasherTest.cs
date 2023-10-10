using ReadingClub.Infrastructure.Common.Helpers;

namespace ReadingClub.UnitTests.Infrastructure.Common.Helpers
{
    public class PasswordHasherTest
    {
        #region GenerateSalt Tests
        [Theory]
        [InlineData(1)]
        [InlineData(16)]
        public void GenerateSalt_WithValidInputParameter_ReturnsNonEmptyString(int saltLength)
        {
            #region Arrange
            #endregion

            #region Act
            var salt = PasswordHasher.GenerateSalt(saltLength);
            #endregion

            #region Assert
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
            Assert.IsType<string>(salt);
            #endregion
        }

        [Fact]
        public void GenerateSalt_WithoutInputParameter_ReturnsNonEmptyString()
        {
            #region Arrange
            #endregion

            #region Act
            var salt = PasswordHasher.GenerateSalt();
            #endregion

            #region Assert
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
            Assert.IsType<string>(salt);
            #endregion
        }
        #endregion

        #region HashPassword Tests
        [Fact]
        public void HashPassword_WithValidInput_ReturnsNonEmptyString()
        {
            #region Arrange
            var password = "testPassword";
            var saltAsBase64String = "dGVzdCBzYWx0"; // base64String representation for "test salt"
            #endregion

            #region Act
            var hashedPassword = PasswordHasher.HashPassword(password, saltAsBase64String);
            #endregion

            #region Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
            Assert.IsType<string>(hashedPassword);
            #endregion
        }

        [Fact]
        public void HashPassword_WithValidInput_ReturnDifferentHashes()
        {
            #region Arrange
            var password = "testPassword";
            var salt_1_AsBase64String = "dGVzdCBzYWx0"; // base64String representation for "test salt"
            var salt_2_AsBase64String = "YW5vdGhlciB0ZXN0IHNhbHQ="; // base64String representation for "another test salt"
            #endregion

            #region Act
            var hashedPassword1 = PasswordHasher.HashPassword(password, salt_1_AsBase64String);
            var hashedPassword2 = PasswordHasher.HashPassword(password, salt_2_AsBase64String);
            #endregion

            #region Assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
            #endregion
        }
        #endregion
    }
}
