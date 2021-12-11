using Xunit;

namespace Autobarn.Website.Tests
{
    public class WebTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> factory;

        public WebTests(TestWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async void WebsiteWorks()
        {
            System.Net.Http.HttpClient client = factory.CreateClient();
            System.Net.Http.HttpResponseMessage response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }
    }
}