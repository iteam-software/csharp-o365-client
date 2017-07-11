using Xunit;

namespace iTEAMConsulting.O365.Tests
{
    public class O365AuthenticationOptionsTests
    {
        [Fact]
        public void ItShouldSetAndGetOptions()
        {
            // Arrange
            var options = new O365AuthenticationOptions
            {
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                TenantId = "TenantId",
                TenantName = "TenantName"
            };

            // Assert
            Assert.Equal("ClientId", options.ClientId);
            Assert.Equal("ClientSecret", options.ClientSecret);
            Assert.Equal("TenantId", options.TenantId);
            Assert.Equal("TenantName", options.TenantName);
        }
    }
}
