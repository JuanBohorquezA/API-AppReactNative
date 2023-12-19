namespace API.Middlewares
{
    public class SignUp
    {
        private readonly string? _ApiKeyLogin;
        public SignUp(IConfiguration configuration)
        {
            _ApiKeyLogin = configuration.GetSection("APIKEY")["signUp"];
        }
        public bool IsValidApiKey(string apikey)
        {
            try
            {
                if (apikey == null) { return false; }
                if (apikey != _ApiKeyLogin) { return false; }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
