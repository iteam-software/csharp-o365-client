using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

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

        [Fact]
        public async void LoginShould_ReturnLoginResponse()
        {
            // Arrange
            var authenticationResult = new Mock<IAuthenticationResultAdapter>();
            authenticationResult.Setup(result => result.AccessToken)
                .Returns("Access Token");
            var authenticationContext = new Mock<IAuthenticationContextAdapter>();
            authenticationContext.Setup(context => context.AcquireTokenAsync(It.IsAny<string>(), It.IsAny<ClientCredential>()))
                .Returns(Task.FromResult(authenticationResult.Object));
            var mockAdalFactory = new Mock<IAdalFactory>();
            mockAdalFactory.Setup(factory => factory.CreateAuthenticationContext(It.IsAny<string>()))
                .Returns(authenticationContext.Object);
            var client = new O365Client(
                MockOptions(),
                mockAdalFactory.Object,
                new BackchannelFactory(),
                MockLoggerFactory());

            // Act
            var response = await client.Login("resource", "clientId", "clientSecret");
            
            // Assert
            Assert.NotNull(response);
            Assert.IsType<LoginResponse>(response);
            Assert.Equal("Access Token", response.AccessToken);
        }

        [Fact]
        public async void LoginShould_LogOnError()
        {
            // Arrange
            var authenticationContext = new Mock<IAuthenticationContextAdapter>();
            authenticationContext.Setup(context => context.AcquireTokenAsync(It.IsAny<string>(), It.IsAny<ClientCredential>()))
                .ThrowsAsync(new Exception());
            var adalFactory = new Mock<IAdalFactory>();
            adalFactory.Setup(factory => factory.CreateAuthenticationContext(It.IsAny<string>()))
                .Returns(authenticationContext.Object);
            var logger = new Mock<ILogger<O365Client>>();
            logger.Setup(i => i.Log(
                Microsoft.Extensions.Logging.LogLevel.Error,
                0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()))
                .Verifiable();
            var client = new O365Client(
                MockOptions(),
                adalFactory.Object,
                new BackchannelFactory(),
                MockLoggerFactory(logger.Object));

            // Act
            var response = await client.Login("resource", "clientId", "clientSecret");

            // Assert
            logger.Verify(x => x.Log(
                Microsoft.Extensions.Logging.LogLevel.Error,
                0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()),
                Times.Once(),
                "Log not called once.");
            Assert.IsType<LoginResponse>(response);
            Assert.Empty(response.AccessToken);
        }
    }
}
