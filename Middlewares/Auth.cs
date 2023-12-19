namespace API.Middlewares
{
    public class Auth
    {
        private readonly string? _ApiKeyLogin;
        private readonly string? _ApiKeySignUp;
        public Auth(IConfiguration configuration) 
        {
            _ApiKeyLogin = configuration.GetSection("APIKEY")["login"];
            _ApiKeySignUp = configuration.GetSection("APIKEY")["signUp"];
        }
        public bool IsValidApiKeyLogin(string apikey)
        {
            try
            {
                if(apikey == null) { return false; }
                if(apikey != _ApiKeyLogin) { return false; }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public bool IsValidApiKeySignUp(string apikey)
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
