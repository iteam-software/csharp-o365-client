using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace iTEAMConsulting.O365.Tests
{
    public class O365ClientTests
    {
        private IOptions<O365AuthenticationOptions> MockOptions()
        {
            var options = new Mock<IOptions<O365AuthenticationOptions>>();
            options.Setup(i => i.Value)
                .Returns(
                    new O365AuthenticationOptions()
                    {
                        ClientId = "LKAMFLDSNFLASDFLANDF",
                        ClientSecret = "AKLSJFLASNDFLASNDF",
                        TenantId = "ATAETAERADFADFGER",
                        TenantName = "FASLALKDFNLANDF"
                    }
                );
            return options.Object;
        }

        private ILoggerFactory MockLoggerFactory(ILogger<O365Client> logger = null)
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(logger);
            return loggerFactory.Object;
        }

        [Fact]
        public void ItShouldInstantiate_O365Client()
        {
            Assert.NotNull(new O365Client(
                MockOptions(),
                new AdalFactory(),
                new BackchannelFactory(),
                MockLoggerFactory()));
        }

        [Fact]
        public void LoginShouldThrowOn_NullResource()
        {
            // Arrange
            var client = new O365Client(
                MockOptions(),
                new AdalFactory(),
                new BackchannelFactory(),
                MockLoggerFactory());

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login(null, "clientId", "clientSecret");
                });
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login("", "clientId", "clientSecret");
                });
        }

        [Fact]
        public void LoginShouldThrowOn_NullClientId()
        {
            // Arrange
            var client = new O365Client(
                MockOptions(),
                new AdalFactory(),
                new BackchannelFactory(),
                MockLoggerFactory());

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login("resource", null, "clientSecret");
                });
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login("resource", "", "clientSecret");
                });
        }

        [Fact]
        public void LoginShouldThrowOn_NullClientSecret()
        {
            // Arrange
            var client = new O365Client(
                MockOptions(),
                new AdalFactory(),
                new BackchannelFactory(),
                MockLoggerFactory());

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login("resource", "clientId", null);
                });
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.Login("resource", "clientId", "");
                });
        }
    }
}
