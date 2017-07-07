namespace iTEAMConsulting.O365
{
    public interface IAdalFactory
    {
        IAuthenticationContextAdapter CreateAuthenticationContext(string authority);
    }
}
