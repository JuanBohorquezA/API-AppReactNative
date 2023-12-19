using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Data;
using System.Net;
using API.Middlewares;
using System.Buffers.Text;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [EnableCors("RuleCores")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerParent
    {

        public readonly string con;
        public readonly AuthRepository _AuthRepository;
        public readonly Encryption _encryption;
        private readonly Auth _Middleware;

        public AuthController(AuthRepository authRepository,Encryption encryption,Auth MiddlewareLogin, IConfiguration configuration)
        {
            _Middleware = MiddlewareLogin;
            _AuthRepository = authRepository;
            _encryption = encryption;
            con = configuration.GetConnectionString("Connection");

        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp ([FromHeader(Name = "API-KEY-ID-VALUE-SIGN_UP")] string apikey,[FromBody] UserSignUp signUp)
        {
            try
            {
                if (!_Middleware.IsValidApiKeySignUp(apikey))
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.Unauthorized, "Api key is not valid", null);
                }
                if (string.IsNullOrEmpty(signUp.Username) || string.IsNullOrEmpty(signUp.Email) || string.IsNullOrEmpty(signUp.Password))
                {
                    return await GetResponseAsync<object?>( HttpStatusCode.BadRequest,"incomplete Data" , null);
                }
                if (!IsBase64String(signUp.Username, signUp.Password))
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, "Username and/or password incorrect format", null);
                }
                var desEncrypt  = await _encryption.DesEncryptRSA(signUp.Username,signUp.Password);
                
                signUp.Username = desEncrypt[0];
                signUp.Password = desEncrypt[1];

                var validationUser = await ValidationUsers(signUp.Username, signUp.Email);
                    
                if(validationUser != null) 
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, "Error", validationUser);
                }
                var result =  _AuthRepository.Registrer(signUp.Username, signUp.Email, signUp.Password);
                if (result.Exception != null) 
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, result.Exception.Message, null);
                }
                return await GetResponseAsync<object?>(HttpStatusCode.OK, "Success", null);
            }
            catch (Exception ex)
            {
                return await GetResponseAsync<object?>(HttpStatusCode.InternalServerError, ex.Message, null);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult>Login([FromHeader(Name = "API-KEY-ID-VALUE-LOGIN")] string keyLogin,[FromBody]UserLogin Login)
        {
            try
            {
                if (!_Middleware.IsValidApiKeyLogin(keyLogin))
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.Unauthorized, "Api key is not valid", null);
                }
                if (string.IsNullOrEmpty(Login.Username) || string.IsNullOrEmpty(Login.Password))
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, "incomplete data", null);
                }
                if (!IsBase64String(Login.Username, Login.Password))
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, "Username and/or password incorrect format", null);
                }
                var desEncrypt = await _encryption.DesEncryptRSA(Login.Username, Login.Password);
                Login.Username = desEncrypt[0];
                Login.Password = desEncrypt[1];

                List<User> UserList = await GetAllUsers();

                var ValidUser = UserList.FirstOrDefault(user => { return (user.Username == Login.Username) && (user.PasswordHash == Login.Password); });
                if (ValidUser == null)
                {
                    return await GetResponseAsync<object?>(HttpStatusCode.BadRequest, "Username and/or password incorrect.", null);
                }
                return await GetResponseAsync<object?>(HttpStatusCode.OK, "Success", null);
            }
            catch (Exception ex)
            {
                return await GetResponseAsync<object?>(HttpStatusCode.InternalServerError, ex.Message, null);
            }
        }


        #region METODOS VALIDACION
        private bool IsBase64String(string Username, string Password)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) { return false; }

            if ((Username.Length % 4 == 0 && Regex.IsMatch(Username, @"^[a-zA-Z0-9\+/]*={0,2}$")) && (Password.Length % 4 == 0 && Regex.IsMatch(Password, @"^[a-zA-Z0-9\+/]*={0,2}$")))
            {
                return true;
            }
            return false;
        }
        private async Task<List<User>> GetAllUsers()
        {
            try
            {
                List<User> Users = await _AuthRepository.GetUsersAsync();
                return Users;
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        private async Task<List<string>?> ValidationUsers(string Username, string Email)
        {
            try
            {
                var result = new List<string>();
                List<User> userList = await GetAllUsers();

                var ExistUser = userList.FirstOrDefault(user => user.Username == Username);
                var ExistEmail = userList.FirstOrDefault(user => user.Email == Email);
                if (ExistUser != null && ExistEmail != null)
                {
                    result.Add("*UserName already exists*");
                    result.Add("*Email already exists*");
                    return result;
                }

                List<string>? validationUser = userList.Select(user =>
                {

                    if (user.Username == Username && user.Email == Email)
                    {
                        result.Add("UserName already exists");
                        result.Add("Email already exists");
                    }
                    else
                    {
                        if (user.Username == Username) result.Add("UserName already exists");
                        if (user.Email == Email) result.Add("Email already exists");
                    }
                    return result;
                }).FirstOrDefault(result => result.Count > 0);

                return validationUser;
            }
            catch
            {
                return new List<string>();
            }
        }

        #endregion
    }
}
