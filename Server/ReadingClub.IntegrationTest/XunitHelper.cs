namespace ReadingClub.IntegrationTest
{
    public class XunitHelper : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;

        public XunitHelper(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task DisposeAsync()
        {
            await _factory.DisposeAsync();
        }

        // Called before any tests run in this class
        public async Task InitializeAsync()
        {
            await TestHelper.DeleteTables();
        }

        // this will make call InitializeAsync() that will drop test database tables before running tests
        [Fact]
        public async Task SomeTest()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/some-endpoint");
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
