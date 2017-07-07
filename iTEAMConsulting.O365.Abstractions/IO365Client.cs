using System.Threading;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365.Abstractions
{
    public interface IO365Client
    {
        /// <summary>
        /// Convenience method to login for the office 365 mail resource.
        /// </summary>
        /// <returns>A login task.</returns>
        Task IntializeForAppMail();

        /// <summary>
        /// Log this O365 client in so that it can access the given resource.
        /// </summary>
        /// <param name="resource">The resource the client will access.</param>
        /// <param name="clientId">The AD App client Id to use.</param>
        /// <param name="clientSecret">The AD App client secret to use.</param>
        /// <returns>The login response task.</returns>
        Task<ILoginResponse> Login(string resource, string clientId, string clientSecret);

        /// <summary>
        /// Send the given message using the this client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="saveToSent">If true, the message sent will be stored in the sent folder for the currently logged in (or impersonated) user.</param>
        /// <param name="cancel">Thread cancellation token.</param>
        /// <returns>The api response task.</returns>
        Task<IApiResponse> SendEmail(IMessage message, bool saveToSent, CancellationToken cancel);
    }
}
