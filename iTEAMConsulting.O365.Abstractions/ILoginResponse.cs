namespace iTEAMConsulting.O365.Abstractions
{
    public interface ILoginResponse : IApiResponse
    {
        string AccessToken { get; }
    }
}
