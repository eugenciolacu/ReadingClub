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
            // Arrange

            //Act
            var salt = PasswordHasher.GenerateSalt(saltLength);

            // Assert
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
            Assert.IsType<string>(salt);
        }

        [Fact]
        public void GenerateSalt_WithoutInputParameter_ReturnsNonEmptyString()
        {
            // Arrange

            // Act
            var salt = PasswordHasher.GenerateSalt();

            // assert
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
            Assert.IsType<string>(salt);
        }
        #endregion

        #region HashPassword Tests
        [Fact]
        public void HashPassword_WithValidInput_ReturnsNonEmptyString()
        {
            // Arrange
            var password = "testPassword";
            var salt = "someSalt"; // base64String representation for "test salt"
            
            // Act
            var hashedPassword = PasswordHasher.HashPassword(password, salt);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
            Assert.IsType<string>(hashedPassword);
        }

        [Fact]
        public void HashPassword_WithValidInput_ReturnDifferentHashes()
        {
            // Arrange
            var password = "testPassword";
            var salt_1 = "someSalt";
            var salt_2 = "someSaltSomeSalt";

            // Act
            var hashedPassword1 = PasswordHasher.HashPassword(password, salt_1);
            var hashedPassword2 = PasswordHasher.HashPassword(password, salt_2);

            // Assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }
        #endregion
    }
}
