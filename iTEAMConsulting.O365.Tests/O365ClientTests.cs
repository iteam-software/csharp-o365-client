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
            options.Setup(i => i.Value).Returns(
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
            var mockLogger = new Mock<ILoggerFactory>();
            mockLogger.Setup(factory => factory.CreateLogger<O365Client>())
                .Returns(logger);
            return mockLogger.Object;
        }
    }
}
