using ReadingClub.SpecFlow.Support.Utilities;

namespace ReadingClub.SpecFlow.Support.Hooks
{
    [Binding]
    public class HostHooks
    {
        private static CustomWebApplicationFactory _customWebApplicationFactory = null!;
        private static HttpClient _httpClient = null!;

        [BeforeTestRun]
        public static async Task BeforeTestRunAsync()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();

            await TestHelper.DeleteTables();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory.Dispose();
        }
    }
}
