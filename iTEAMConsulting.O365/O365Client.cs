using iTEAMConsulting.O365.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public class O365Client : IO365Client
    {
        private readonly ILogger _logger;
        private readonly IAdalFactory _adalFactory;
        private readonly O365AuthenticationOptions _options;
        private readonly IHttpClientAdapter _backchannel;
        private string _apiBaseUrl = "/api/v2.0/users/";
        private string _accessToken;

        /// <summary>
        /// Construct a new O365Client.
        /// </summary>
        /// <param name="optionsAccessor">Provides access to the O365AuthenticationOptions configuration object.</param>
        /// <param name="adalFactory">Creates ADAL Authentication contexts.</param>
        /// <param name="backchannelFactory">Creates HttpClient backchannels</param>
        /// <param name="loggerFactory">Creates Loggers.</param>
        public O365Client(
            IOptions<O365AuthenticationOptions> optionsAccessor,
            IAdalFactory adalFactory,
            IBackchannelFactory backchannelFactory,
            ILoggerFactory loggerFactory)
        {
            _options = optionsAccessor.Value ?? new O365AuthenticationOptions();
            _logger = loggerFactory.CreateLogger(nameof(O365Client));
            _adalFactory = adalFactory;
            _backchannel = backchannelFactory.CreateBackchannel("https://outlook.office.com");
            _apiBaseUrl += _options.FromAddress;
        }

        /// <summary>
        /// Backchannel used to access the Office365 endpoints.
        /// </summary>
        public IHttpClientAdapter Backchannel => _backchannel;

        /// <summary>
        /// Convenience method to login for the office 365 mail resource.
        /// </summary>
        /// <returns>A login task.</returns>
        public async Task IntializeForAppMail()
        {
            if (!string.IsNullOrEmpty(_options.CertThumbprint))
            {
                await LoginWithThumbprint("https://outlook.office.com", _options.ClientId, _options.CertThumbprint);
            }
            else
            {
                await Login("https://outlook.office.com", _options.CertBytes, _options.CertPrivateKey, _options.ClientId);
            }
        }

        private async Task<LoginResponse> LoginWithThumbprint(string resource, string clientId, string thumbprint)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(thumbprint))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            var context = _adalFactory.CreateAuthenticationContext("https://login.microsoftonline.com/" + _options.TenantName);
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, _options.CertThumbprint, false);
            if (certs.Count == 0)
            {
                throw new Exception("Unable to find certificate for the given thumbprint");
            }

            var cert = certs[0];
            var assertion = new ClientAssertionCertificate(clientId, cert);

            try
            {
                var response = await context.AcquireTokenAsync(resource, assertion);
                _accessToken = response.AccessToken;
                return new LoginResponse(_accessToken);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Failed to Login to Active Directory");
                return new LoginResponse(string.Empty);
            }
        }

        public async Task<ILoginResponse> Login(string resource, byte[] certBytes, string secret, string clientId)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (!certBytes.Any())
            {
                throw new ArgumentNullException(nameof(certBytes));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            var context = _adalFactory.CreateAuthenticationContext("https://login.microsoftonline.com/" + _options.TenantName);
            var cert = new X509Certificate2(certBytes, secret, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            var assertion = new ClientAssertionCertificate(clientId, cert);

            try
            {
                var response = await context.AcquireTokenAsync(resource, assertion);
                _accessToken = response.AccessToken;
                return new LoginResponse(_accessToken);
            } catch (Exception e)
            {
                _logger.LogError(0, e, "Failed to Login to Active Directory");
                return new LoginResponse(string.Empty);
            }
        }

        /// <summary>
        /// Log this O365 client in so that it can access the given resource.
        /// </summary>
        /// <param name="resource">The resource the client will access.</param>
        /// <param name="clientId">The AD App client Id to use.</param>
        /// <param name="clientSecret">The AD App client secret to use.</param>
        /// <returns>The login response task.</returns>
        public async Task<ILoginResponse> Login(string resource, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            var context = _adalFactory.CreateAuthenticationContext("https://login.microsoftonline.com/" + _options.TenantName);
            var credential = new ClientCredential(_options.ClientId, _options.ClientSecret);

            // We may need a UserAssertion here to login on behalf of a user with a mailbox.

            try
            {
                var response = await context.AcquireTokenAsync(resource, credential);
                _accessToken = response.AccessToken;
                return new LoginResponse(_accessToken);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Failed to Login to Active Directory.");
                return new LoginResponse(string.Empty);
            }
        }

        /// <summary>
        /// Send the given message using the this client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="saveToSent">If true, the message sent will be stored in the sent folder for the currently logged in (or impersonated) user.</param>
        /// <param name="cancel">Thread cancellation token.</param>
        /// <returns>The api response task.</returns>
        public async Task<IApiResponse> SendEmail(IMessage message, bool saveToSent, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(_accessToken))
            {
                throw new InvalidOperationException("You must login before sending email");
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(message.Subject))
            {
                throw new ArgumentNullException(nameof(message.Subject));
            }

            if (string.IsNullOrEmpty(message.Body))
            {
                throw new ArgumentNullException(nameof(message.Subject));
            }

            if (
                message.ToRecipients.Count() == 0 ||
                message.ToRecipients.Any(r => string.IsNullOrEmpty(r.EmailAddress))
            )
            {
                throw new ArgumentException("You must provide valid recipient email addresses", nameof(message.ToRecipients));
            }

            // Construct the payload
            var data = JsonConvert.SerializeObject(new
            {
                Message = new
                {
                    Subject = message.Subject,
                    Body = new
                    {
                        ContentType = "Html",
                        Content = message.Body,
                    },
                    ToRecipients = message.ToRecipients.Select(r => new { EmailAddress = new { Address = r.EmailAddress } }),
                },
                SaveToSentItems = saveToSent,
            });
            var payload = new StringContent(data, Encoding.UTF8, "application/json");

            // Create the message
            var request = new HttpRequestMessage(HttpMethod.Post, _apiBaseUrl + "/sendmail");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("iTEAMConsulting.0365", "1.0.0"));
            request.Headers.Add("client-request-id", new Guid().ToString());
            request.Headers.Date = new DateTimeOffset(DateTime.Now);
            request.Content = payload;

            // Send!
            try
            {
                var response = await Backchannel.SendAsync(request);
                return new ApiResponse((int)response.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Failed to send the message.");
                return new ApiResponse(-1);
            }
        }
    }
}
