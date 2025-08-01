using Newtonsoft.Json;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.SpecFlow.Support;
using System.Net;
using System.Text;

namespace ReadingClub.SpecFlow.StepDefinitions
{
    [Binding]
    public class IntegrationTestsForUserControllerClassStepDefinitions
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;

        private CreateUserDto _createUserDto = null!;
        private HttpContent _content = null!;
        private HttpResponseMessage _response = null!;

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

        [Given(@"create HttpContent for request")]
        public void GivenCreateHttpContentForRequest()
        {
            string json = JsonConvert.SerializeObject(_createUserDto);
            _content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        [Given("an invalid CreateUserDto object")]
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

        [When(@"try create an account")]
        public async Task WhenTryCreateAnAccount()
        {
            _response =await _httpClient.PostAsync("/api/User/create", _content);
        }

        [Then(@"an HttpStatusCode\.OK is returned")]
        public void ThenAnHttpStatusCode_OKIsReturned()
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
        }

        [Then("an HttpStatusCode.BadRequest is returned")]
        public void ThenAnHttpStatusCode_BadRequestIsReturned()
        {
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        }

        [Then(@"the following details for created account are returned")]
        public async Task ThenTheFollowingDetailsAreReturned(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (UserDto)null! });

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

        [Then("the following details of errors for created account with invalid fields DTO are")]
        public async Task ThenTheFollowingDetailsOfErrorsForCreatedAccountWithInvalidFieldsDTOAre(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });

            Assert.Equal(int.Parse(table.Rows[0]["ErrorCount"]), resultContent?.errors.Count);
            Assert.Equal(table.Rows[0]["EmailError"], resultContent?.errors["Email"][0]);
            Assert.Equal(table.Rows[0]["ConfirmPasswordError"], resultContent?.errors["ConfirmPassword"][0]);
        }

    }
}
