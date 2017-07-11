using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using iTEAMConsulting.O365.Abstractions;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections;

namespace iTEAMConsulting.O365.Tests
{
    public class O365ClientTests
    {
        [Fact]
        public void ItShouldInstantiate_O365Client()
        {
            var client = CreateClient(
                adal: new AdalFactory(),
                backchannel: new BackchannelFactory()
            );

            Assert.NotNull(client);
        }

        [Fact]
        public void LoginShouldThrowOn_NullResource()
        {
            // Arrange
            var client = CreateClient(
                adal: new AdalFactory(),
                backchannel: new BackchannelFactory()
            );

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
            var client = CreateClient(
                adal: new AdalFactory(),
                backchannel: new BackchannelFactory()
            );

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
            var client = CreateClient(
                adal: new AdalFactory(),
                backchannel: new BackchannelFactory()
            );

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
        public async void LoginShould_LogOnError()
        {
            // -------------------- Arrange --------------------
            // ADAL Factory
            var authenticationContext = new Mock<IAuthenticationContextAdapter>();
            authenticationContext.Setup(context => context.AcquireTokenAsync(
                It.IsAny<string>(),
                It.IsAny<ClientCredential>()
            ))
                .ThrowsAsync(new Exception());
            var adalFactory = new Mock<IAdalFactory>();
            adalFactory.Setup(factory => factory.CreateAuthenticationContext(It.IsAny<string>()))
                .Returns(authenticationContext.Object);

            // Logger
            var logger = new Mock<ILogger<O365Client>>();
            logger.Setup(i => i.Log(
                Microsoft.Extensions.Logging.LogLevel.Error,
                0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()))
                .Verifiable();

            // Create Client
            var client = CreateClient(
                adal: adalFactory.Object,
                backchannel: new BackchannelFactory(),
                logger: MockLoggerFactory(logger.Object));

            // -------------------- Act --------------------
            var response = await client.Login("resource", "clientId", "clientSecret");

            // -------------------- Assert --------------------
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

        [Fact]
        public async void LoginShould_ReturnLoginResponse()
        {
            // Arrange
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            var response = await client.Login("resource", "clientId", "clientSecret");
            
            // Assert
            Assert.NotNull(response);
            Assert.IsType<LoginResponse>(response);
            Assert.Equal("Access Token", response.AccessToken);
        }

        [Fact]
        public void SendEmailShouldThrow_IfNoAccessKey()
        {
            // Arrange
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.Subject)
                .Returns("Subject");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient>());
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(
                adal: new AdalFactory(),
                backchannel: new BackchannelFactory()
            );

            // Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                {
                    await client.SendEmail(
                        message.Object,
                        false,
                        token
                    );
                });
        }

        [Fact]
        public async void SendEmailsShouldThrowOn_NullMessage()
        {
            // Arrange
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            await client.Login("resource", "clientId", "clientSecret");

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.SendEmail(
                        null,
                        false,
                        token
                    );
                }
            );
        }

        [Fact]
        public async void SendEmailsShouldThrowOn_NullMessageSubject()
        {
            // Arrange
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient>());
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            await client.Login("resource", "clientId", "clientSecret");

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.SendEmail(
                        message.Object,
                        false,
                        token
                    );
                }
            );
        }

        [Fact]
        public async void SendEmailsShouldThrowOn_NullMessageBody()
        {
            // Arrange
            var message = new Mock<IMessage>();
            message.Setup(m => m.Subject)
                .Returns("Subject");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient>());
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            await client.Login("resource", "clientId", "clientSecret");

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await client.SendEmail(
                        message.Object,
                        false,
                        token
                    );
                }
            );
        }

        [Fact]
        public async void SendEmailsShouldThrowOn_NullMessageRecipients()
        {
            // Arrange
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.Subject)
                .Returns("Subject");
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            await client.Login("resource", "clientId", "clientSecret");

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                {
                    await client.SendEmail(
                        message.Object,
                        false,
                        token
                    );
                }
            );
        }

        [Fact]
        public async void SendEmailsShouldThrowOn_InvalidMessageRecipients()
        {
            // Arrange
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.Subject)
                .Returns("Subject");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient>());
            CancellationToken token = new CancellationTokenSource().Token;
            var client = CreateClient(backchannel: new BackchannelFactory());

            // Act
            await client.Login("resource", "clientId", "clientSecret");

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                {
                    await client.SendEmail(
                        message.Object,
                        false,
                        token
                    );
                }
            );
        }

        [Fact]
        public async void SendEmailsShouldLog_OnUnsuccessfulSendAsync()
        {
            // -------------------- Arrange --------------------
            // Logger
            var logger = new Mock<ILogger<O365Client>>();
            logger.Setup(i => i.Log(
                Microsoft.Extensions.Logging.LogLevel.Error,
                0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));

            // Backchannel
            var http = new Mock<IHttpClientAdapter>();
            http.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ThrowsAsync(new Exception());
            var backchannelFactory = new Mock<IBackchannelFactory>();
            backchannelFactory.Setup(b => b.CreateBackchannel(It.IsAny<string>()))
                .Returns(http.Object);

            // Create the client
            var client = CreateClient(
                logger: MockLoggerFactory(logger.Object),
                backchannel: backchannelFactory.Object
            );

            // Message (Subject, Body, and Recipients) and Cancellation Token
            var recipient = new Mock<IRecipient>();
            recipient.Setup(r => r.EmailAddress)
                .Returns("abc@abc.com");
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.Subject)
                .Returns("Subject");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient> { recipient.Object });
            CancellationToken token = new CancellationTokenSource().Token;

            // -------------------- Act --------------------
            var loginResponse = await client.Login("resource", "clientId", "clientSecret");
            var apiResponse = await client.SendEmail(
                message.Object,
                false,
                token
            );

            // -------------------- Assert --------------------
            logger.Verify(x => x.Log(
                Microsoft.Extensions.Logging.LogLevel.Error,
                0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()),
                Times.Once(),
                "Log not called once.");
            Assert.IsType<ApiResponse>(apiResponse);
        }

        [Fact]
        public async void SendEmailsShould_ReturnApiResponse()
        {
            // -------------------- Arrange --------------------
            // Create the client
            var client = CreateClient();

            // Message (Subject, Body, and Recipients) and Cancellation Token
            var recipient = new Mock<IRecipient>();
            recipient.Setup(r => r.EmailAddress)
                .Returns("abc@abc.com");
            var message = new Mock<IMessage>();
            message.Setup(m => m.Body)
                .Returns("Body");
            message.Setup(m => m.Subject)
                .Returns("Subject");
            message.Setup(m => m.ToRecipients)
                .Returns(new List<IRecipient> { recipient.Object });
            CancellationToken token = new CancellationTokenSource().Token;

            // -------------------- Act --------------------
            var loginResponse = await client.Login("resource", "clientId", "clientSecret");
            var apiResponse = await client.SendEmail(
                message.Object,
                false,
                token
            );

            // -------------------- Assert --------------------
            Assert.IsType<ApiResponse>(apiResponse);
            Assert.Equal(200, apiResponse.StatusCode);
        }

        /* ------------------------------ HELPER FUNCTIONS ------------------------------ */
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

        private IO365Client CreateClient(
            IOptions<O365AuthenticationOptions> options = null,
            IAdalFactory adal = null,
            IBackchannelFactory backchannel = null,
            ILoggerFactory logger = null
        )
        {
            if (options == null)
            {
                options = MockOptions();
            }

            if (adal == null)
            {
                var authenticationResult = new Mock<IAuthenticationResultAdapter>();
                authenticationResult.Setup(result => result.AccessToken)
                    .Returns("Access Token");
                var authenticationContext = new Mock<IAuthenticationContextAdapter>();
                authenticationContext
                    .Setup(context => context.AcquireTokenAsync(
                        It.IsAny<string>(),
                        It.IsAny<ClientCredential>()
                    ))
                    .Returns(Task.FromResult(authenticationResult.Object));
                var adalFactory = new Mock<IAdalFactory>();
                adalFactory.Setup(factory => factory.CreateAuthenticationContext(It.IsAny<string>()))
                    .Returns(authenticationContext.Object);
                adal = adalFactory.Object;
            }

            if (backchannel == null)
            {
                var http = new Mock<IHttpClientAdapter>();
                http.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
                var backchannelFactory = new Mock<IBackchannelFactory>();
                backchannelFactory.Setup(b => b.CreateBackchannel(It.IsAny<string>()))
                    .Returns(http.Object);
                backchannel = backchannelFactory.Object;
            }

            if (logger == null)
            {
                logger = MockLoggerFactory();
            }

            return new O365Client(
                options,
                adal,
                backchannel,
                logger
            );
        }
    }
}
