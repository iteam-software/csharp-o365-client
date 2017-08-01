using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace iTEAMConsulting.O365.Tests
{
    public class AdalFactoryTests
    {
        [Fact]
        public void AdalFactory_Instantiates()
        {
            // Act
            var adal = new AdalFactory();

            // Assert
            Assert.NotNull(adal);
        }

        [Fact]
        public void CreateAuthenticationContext_ReturnsContext()
        {
            // Arrange
            var adal = new AdalFactory();

            // Act
            var context = adal.CreateAuthenticationContext("https://abc.com/authority");

            // Assert
            Assert.IsType<AuthenticationContextAdapter>(context);
        }
    }
}
