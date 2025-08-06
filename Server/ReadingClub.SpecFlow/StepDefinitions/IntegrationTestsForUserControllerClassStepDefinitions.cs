using Gherkin;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.SpecFlow.Support;
using ReadingClub.SpecFlow.Support.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TechTalk.SpecFlow.CommonModels;

namespace ReadingClub.SpecFlow.StepDefinitions
{
    [Binding]
    public class IntegrationTestsForUserControllerClassStepDefinitions
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;

        private CreateUserDto _createUserDto = null!;
        private UserLoginDto _userLoginDto = null!;
        private HttpContent _content = null!;
        private HttpResponseMessage _response = null!;
        private string? _token = null!;
        private UserDto _userDto = null!;
        private UpdateUserDto _updateUserDto = null!;
        private User _user = null!;

        public IntegrationTestsForUserControllerClassStepDefinitions()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();
        }

        [Given(@"a valid CreateUserDto object")]
        public void GivenAValidCreateUserDtoObject(Table table)
        {
            _createUserDto = new CreateUserDto
            {
                UserName = table.Rows[0]["UserName"],
                Email = table.Rows[0]["Email"],
                Password = table.Rows[0]["Password"],
                ConfirmPassword = table.Rows[0]["ConfirmPassword"]
            };
        }

        [Given(@"an empty CreateUserDto object")]
        public void GivenAnEmptyCreateUserDtoObject()
        {
            _createUserDto = new CreateUserDto() { };
        }

        [Given(@"create HttpContent for create request")]
        public void GivenCreateHttpContentForCreateRequest()
        {
            string json = JsonConvert.SerializeObject(_createUserDto);
            _content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        [Given(@"an invalid CreateUserDto object")]
        public void GivenAnInvalidCreateUserDtoObject(Table table)
        {
            _createUserDto = new CreateUserDto()
            {
                UserName = table.Rows[0]["UserName"],
                Email = table.Rows[0]["Email"],
                Password = table.Rows[0]["Password"],
                ConfirmPassword = table.Rows[0]["ConfirmPassword"]
            };
        }

        [Given(@"an valid user in database instantiate an CreateUserDto with the same username")]
        public async Task GivenAnValidUserInDatabaseInstantiateAnCreateUserDtoWithTheSameUsername()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _createUserDto = new CreateUserDto()
            {
                UserName = user.UserName,
                Email = "another" + user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };
        }

        [Given(@"an valid user in database instantiate an CreateUserDto with the same email")]
        public async Task GivenAnValidUserInDatabaseInstantiateAnCreateUserDtoWithTheSameEmailAsync()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _createUserDto = new CreateUserDto()
            {
                UserName = "another" + user.UserName,
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };
        }

        [Given(@"an valid user in database instantiate an CreateUserDto with the same username and email")]
        public async Task GivenAnValidUserInDatabaseInstantiateAnCreateUserDtoWithTheSameUsernameAndEmailAsync()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _createUserDto = new CreateUserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };
        }

        [Given(@"an empty UserLoginDto")]
        public void GivenAnEmptyUserLoginDto()
        {
            _userLoginDto = new UserLoginDto();
        }

        [Given(@"create HttpContent for login request")]
        public void GivenCreateHttpContentForLoginRequest()
        {
            string json = JsonConvert.SerializeObject(_userLoginDto);
            _content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        [Given(@"a valid user in database login with its credentials")]
        public async Task GivenAValidUserInDatabaseLoginWithItsCredentials()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _userLoginDto = new UserLoginDto()
            {
                Email = user.Email,
                Password = TestHelper.OriginalPassword
            };
        }

        [Given(@"invalid credentials in UserLoginDto")]
        public void GivenInvalidCredentialsInUserLoginDto()
        {
            _userLoginDto = new UserLoginDto()
            {
                Email = "wrongEmail@test.com",
                Password = "wrongPassword"
            };
        }

        [Given(@"a token generated based on an UserDto object with empty fields")]
        public void GivenATokenGeneratedBasedOnAnUserDtoObjectWithEmptyFields()
        {
            UserDto userDto = new UserDto()
            {
                UserName = "",
                Email = ""
            };

            _token = TestHelper.GenerateToken(userDto);
        }

        [Given(@"create HttpContent for isTokenValid")]
        public void GivenCreateHttpContentForIsTokenValid()
        {
            string json = JsonConvert.SerializeObject(new TokenDto() { Token = _token! });
            _content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        [Given(@"a token generated based on a valid UserDto that exists in database")]
        public async Task GivenATokenGeneratedBasedOnAValidUserDtoThatExistsInDatabaseAsync()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            _token = TestHelper.GenerateToken(_userDto);
        }

        [Given(@"a token generated based on a valid UserDto that exists in database but with expiration less than a day")]
        public async Task GivenATokenGeneratedBasedOnAValidUserDtoThatExistsInDatabaseButWithExpirationLessThanADayAsync()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            _userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            _token = TestHelper.GenerateTokenThithExpirationOfLessThanADay(_userDto);
        }

        [Given(@"a token generated based on a valid UserDto that exists in database but with expiration more than a day")]
        public async Task GivenATokenGeneratedBasedOnAValidUserDtoThatExistsInDatabaseButWithExpirationMoreThanADayAsync()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            _token = TestHelper.GenerateTokenThithExpirationOfMoreThanADay(_userDto);
        }

        [Given(@"an altered token")]
        public async Task GivenAnAlteredTokenAsync()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            _userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            _token = TestHelper.GenerateAlteredToken(_userDto);
        }

        [Given(@"a UserDto object with empty fields")]
        public void GivenAUserDtoObjectWithEmptyFields()
        {
            _userDto = new UserDto()
            {
                UserName = "",
                Email = ""
            };
        }

        [Given(@"generate JWT token and set it in the Authorization header")]
        public void GivenGenerateJWTTokenAndSetItInTheAuthorizationHeader()
        {
            string? token = TestHelper.GenerateToken(_userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Given(@"a UserDto object with inexistent credentials")]
        public void GivenAUserDtoObjectWithInexistentCredentials()
        {
            _userDto = new UserDto()
            {
                UserName = "someValidUserName",
                Email = "someValidEmail@test.com"
            };
        }

        [Given(@"a UserDto object with valid fields, user is in database")]
        public async Task GivenAUserDtoObjectWithValidFieldsUserIsInDatabaseAsync()
        {
            _user = await TestHelper.AddNewUserToTestDatabase();

            _userDto = new UserDto()
            {
                UserName = _user.UserName,
                Email = _user.Email,
            };
        }

        [Given(@"create an UpdateUserDto object with empty fields")]
        public void GivenCreateAnUpdateUserDtoObjectWithEmptyFields()
        {
            _updateUserDto = new UpdateUserDto();
        }

        [Given(@"create HttpContent for update")]
        public void GivenCreateHttpContentForUpdate()
        {
            string jsonWhenEmptyFields = JsonConvert.SerializeObject(_updateUserDto);
            _content = new StringContent(jsonWhenEmptyFields, Encoding.UTF8, "application/json");
        }

        [Given(@"create an invalid UpdateUserDto object")]
        public void GivenCreateAnInvalidUpdateUserDtoObject()
        {
            _updateUserDto = new UpdateUserDto()
            {
                UserName = "Test",
                Email = "Test",
                Password = "Test",
                ConfirmPassword = "Something different",
                OldEmail = _user.Email,
                IsEditPassword = true,
            };
        }

        [Given(@"create an invalid UpdateUserDto object with wrong OldEmail field")]
        public void GivenCreateAnInvalidUpdateUserDtoObjectWithWrongOldEmailField()
        {
            _updateUserDto = new UpdateUserDto()
            {
                UserName = "NewUserName",
                Email = "newEmail@test.com",
                Password = "newPassword",
                ConfirmPassword = "newPassword",
                OldEmail = "wrongOldEmail@test.com",
                IsEditPassword = true
            };
        }

        [Given(@"create a valid UpdateUserDto object")]
        public void GivenCreateAValidUpdateUserDtoObject()
        {
            _updateUserDto = new UpdateUserDto()
            {
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString() + "@test.com",
                Password = "newPassword",
                ConfirmPassword = "newPassword",
                OldEmail = _user.Email,
                IsEditPassword = true
            };
        }

        [When(@"try delete user")]
        public async Task WhenTryDeleteUserAsync()
        {
            _response = await _httpClient.DeleteAsync("/api/User/deleteLoggedUser");
        }

        [When(@"try create an account")]
        public async Task WhenTryCreateAnAccount()
        {
            _response =await _httpClient.PostAsync("/api/User/create", _content);
        }

        [When(@"try login")]
        public async Task WhenTryLoginAsync()
        {
            _response = await _httpClient.PostAsync("/api/User/login", _content);
        }

        [When("try validate token")]
        public async Task WhenTryValidateTokenAsync()
        {
            _response = await _httpClient.PostAsync("/api/User/isTokenValid", _content);
        }

        [When(@"try get user")]
        public async Task WhenTryGetUserAsync()
        {
            _response = await _httpClient.PostAsync("/api/User/getFullDetailsOfLoggedUser", null);
        }

        [When(@"try update user")]
        public async Task WhenTryUpdateUserAsync()
        {
            _response = await _httpClient.PutAsync("/api/User/update", _content);
        }

        [When(@"try get logged user")]
        public async Task WhenTryGetLoggedUserAsync()
        {
            _response = await _httpClient.PostAsync("/api/User/getLoggedUser", null);
        }


        [Then(@"an HttpStatusCode\.OK is returned")]
        public void ThenAnHttpStatusCode_OKIsReturned()
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
        }

        [Then(@"an HttpStatusCode.BadRequest is returned")]
        public void ThenAnHttpStatusCode_BadRequestIsReturned()
        {
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        }

        [Then(@"the following details for created account are returned")]
        public async Task ThenTheFollowingDetailsAreReturned(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Data = (UserDto)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["UserName"], resultContent?.Data.UserName);
            Assert.Equal(table.Rows[0]["Email"], resultContent?.Data.Email);
        }

        [Then(@"the following details of errors for created account with empty DTO are")]
        public async Task ThenTheFollowingDetailsOfErrorsForCreatedAccountWithEmptyDTOAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });

            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["UserNameError"], resultContent?.errors["UserName"][0]);
            Assert.Equal(table.Rows[0]["EmailError0"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["EmailError1"], resultContent?.errors["Email"][1]);
            Assert.Equal(table.Rows[0]["PasswordError"], resultContent?.errors["Password"][0]);
            Assert.Equal(table.Rows[0]["ConfirmPasswordError"], resultContent?.errors["ConfirmPassword"][0]);
        }

        [Then(@"the following details of errors for created account with invalid fields DTO are")]
        public async Task ThenTheFollowingDetailsOfErrorsForCreatedAccountWithInvalidFieldsDTOAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });

            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["EmailError"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["ConfirmPasswordError"], resultContent?.errors["ConfirmPassword"][0]);
        }

        [Then(@"the following details of error for created account with same fields are")]
        public async Task ThenTheFollowingDetailsOfErrorForCreatedAccountWithSameFieldsAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then(@"the following details of errors for login are")]
        public async Task ThenTheFollowingDetailsOfErrorsForLoginAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });

            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["EmailError"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["PasswordError"], resultContent?.errors["Password"][0]);
        }

        [Then(@"a non-null and non-empty token is returned")]
        public async Task ThenANon_NullAndNon_EmptyTokenIsReturnedAsync()
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Data = (string)null! });

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent?.Data);
        }

        [Then(@"the following details of errors for login with inexistent credentials are")]
        public async Task ThenTheFollowingDetailsOfErrorsForLoginWithInexistentCredentialsAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then("following details of error for isTokenValid are")]
        public async Task ThenFollowingDetailsOfErrorForIsTokenValidAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then("returned token is the same, not-null and not-empty")]
        public async Task ThenReturnedTokenIsTheSameNot_NullAndNot_EmptyAsync()
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Data = (string)null! });

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent?.Data);
            Assert.Equal(_token, resultContent?.Data);
        }

        [Then(@"return a new token with updated expiration date, not-null and not-empty")]
        public async Task ThenReturnANewTokenWithUpdatedExpirationDateNot_NullAndNot_Empty()
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Data = (string)null! });

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent?.Data);
            Assert.NotEqual(_token, resultContent?.Data);
        }

        [Then(@"the following details of error for getting user are")]
        public async Task ThenTheFollowingDetailsOfErrorForGettingUserAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then(@"the same user details is returned")]
        public async Task ThenTheSameUserDetailsIsReturnedAsync()
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false, Data = (UserDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(_userDto.UserName, resultContent?.Data.UserName);
            Assert.Equal(_userDto.Email, resultContent?.Data.Email);
        }

        [Then(@"the following details of error for update user with an invalid UpdateUserDto object with empty fields are")]
        public async Task ThenTheFollowingDetailsOfErrorForUpdateUserWithAnInvalidUpdateUserDtoObjectWithEmptyFieldsAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["UserNameError"], resultContent?.errors["UserName"][0]);
            Assert.Equal(table.Rows[0]["EmailError0"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["EmailError1"], resultContent?.errors["Email"][1]);
            Assert.Equal(table.Rows[0]["PasswordError"], resultContent?.errors["Password"][0]);
            Assert.Equal(table.Rows[0]["ConfirmPasswordError"], resultContent?.errors["ConfirmPassword"][0]);
            Assert.Equal(table.Rows[0]["OldEmailError"], resultContent?.errors["OldEmail"][0]);
        }

        [Then(@"the following details of error for update user with invalid UpdateUserDto object are")]
        public async Task ThenTheFollowingDetailsOfErrorForUpdateUserWithInvalidUpdateUserDtoObjectAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["EmailError"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["ConfirmPasswordError"], resultContent?.errors["ConfirmPassword"][0]);
        }

        [Then(@"the following details of error for updated with invalid UpdatedUserDto object, wrong OldEmail field, are")]
        public async Task ThenTheFollowingDetailsOfErrorForUpdatedWithInvalidUpdatedUserDtoObjectWrongOldEmailFieldAreAsync(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then(@"returned data are the same as in UpdateUserDto object")]
        public async Task ThenReturnedDataAreTheSameAsInUpdateUserDtoObjectAsync()
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { NewStatus = false, Data = (UserDto)null! });

            Assert.True(resultContent?.NewStatus);
            Assert.Equal(_updateUserDto.UserName, resultContent?.Data.UserName);
            Assert.Equal(_updateUserDto.Email.ToLower(), resultContent?.Data.Email);
        }

        [Then(@"the following details of error for delete with invalid claims identity are")]
        public async Task ThenTheFollowingDetailsOfErrorForDeleteWithInvalidClaimsIdentityAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }

        [Then(@"the following delete details are")]
        public async Task ThenTheFollowingDeleteDetailsAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, 
                new { Status = false });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
        }

        [Then("the following details of errors for get logged user are")]
        public async Task ThenTheFollowingDetailsOfErrorsForGetLoggedUserAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { Status = false, Message = (string)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["Message"], resultContent?.Message);
        }
    }
}
