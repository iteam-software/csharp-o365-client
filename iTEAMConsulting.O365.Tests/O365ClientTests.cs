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
            var options = MockOptions();
            var loggerFactory = MockLoggerFactory();

            Assert.NotNull(new O365Client(
                options,
                new AdalFactory(),
                new BackchannelFactory(),
                loggerFactory));
        }
    }
}
