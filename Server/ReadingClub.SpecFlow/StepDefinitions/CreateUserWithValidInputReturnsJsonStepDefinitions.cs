using Newtonsoft.Json;
using ReadingClub.Infrastructure.DTO.User;
using ReadingClub.SpecFlow.Support;
using System.Net;
using System.Text;

namespace ReadingClub.SpecFlow.StepDefinitions
{
    [Binding]
    public class CreateUserWithValidInputReturnsJsonStepDefinitions
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;

        private CreateUserDto _createUserDto = null!;
        private HttpContent _content = null!;
        private HttpResponseMessage _response = null!;

        public CreateUserWithValidInputReturnsJsonStepDefinitions()
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
                ConfirmPassword = table.Rows[0]["ConfirmPassword"],
            };
        }

        [Given(@"create HttpContent for request")]
        public void GivenCreateHttpContentForRequest()
        {
            string json = JsonConvert.SerializeObject(_createUserDto);
            _content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        [When(@"create an account")]
        public async Task WhenCreateAnAccount()
        {
            _response =await _httpClient.PostAsync("/api/User/create", _content);
        }

        [Then(@"an HttpStatusCode\.OK is returned")]
        public void ThenAnHttpStatusCode_OKIsReturned()
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
        }


        [Then(@"the following details are returned:")]
        public async Task ThenTheFollowingDetailsAreReturned(Table table)
        {
            var result = await _response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (UserDto)null! });

            Assert.Equal(table.Rows[0]["Status"], resultContent?.Status.ToString());
            Assert.Equal(table.Rows[0]["UserName"], resultContent?.Data.UserName);
            Assert.Equal(table.Rows[0]["Email"], resultContent?.Data.Email);
        }
    }
}
