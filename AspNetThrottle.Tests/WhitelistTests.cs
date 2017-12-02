using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetThrottle.Tests.Fakes;
using Xunit;

namespace AspNetThrottle.Tests
{
    public class WhitelistTests
    {
        public static ThrottleProcessor GetProcessorWithWhitelist(string whitelistedClientId, string whitelistedEndpoint)
        {
            var store = new FakeCounterStore();
            var options = new BasicOptions
            {
                ClientWhitelist = new List<string> { whitelistedClientId },
                EndpointWhitelist = new List<string> { whitelistedEndpoint }
            };
            var matcher = new ClientIdRuleMatcher(options.ClientWhitelist, options.ClientPolicies, options.IdIgnoreCase);
            var processor = new ThrottleProcessor(options, store, matcher);

            return processor;
        }

        [Fact]
        public void IsWhitelisted_Client_Whitelisted()
        {
            // Arrange
            var clientId = "clientA";
            var request = new ClientRequest { ClientId = "ClientA" }; // Different case on purpose.
            var processor = GetProcessorWithWhitelist(clientId, null);

            // Act
            var whitelisted = processor.IsWhitelisted(request);

            // Assert
            Assert.True(whitelisted);
        }

        [Fact]
        public void IsWhitelisted_Client_NotWhitelisted()
        {
            // Arrange
            var clientId = "clientA";
            var request = new ClientRequest { ClientId = clientId };
            var processor = GetProcessorWithWhitelist("dummyId", "dummyPath");

            // Act
            var whitelisted = processor.IsWhitelisted(request);

            // Assert
            Assert.False(whitelisted);
        }

        [Fact]
        public void IsWhitelisted_Endpoint_Whitelisted()
        {
            // Arrange
            var endpoint = "/endpointA";
            var request = new ClientRequest { Path = endpoint };
            var processor = GetProcessorWithWhitelist(null, "*:" + endpoint);

            // Act
            var whitelisted = processor.IsWhitelisted(request);

            // Assert
            Assert.True(whitelisted);
        }

        [Fact]
        public void IsWhitelisted_Endpoint_NotWhitelisted()
        {
            // Arrange
            var endpoint = "/endpointA";
            var request = new ClientRequest { Path = endpoint };
            var processor = GetProcessorWithWhitelist(null, "dummyPath");

            // Act
            var whitelisted = processor.IsWhitelisted(request);

            // Assert
            Assert.False(whitelisted);
        }
    }
}
