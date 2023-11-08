using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        public readonly string con;
        public readonly AuthRepository _AuthRepository;

        public AuthController(AuthRepository authRepository, IConfiguration configuration)
        {
            _AuthRepository = authRepository;
            con = configuration.GetConnectionString("Connection");
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp (UserSignUp signUp)
        {
            Response response = new Response();
            try
            {        
                if (signUp.Username == string.Empty || signUp.Email == string.Empty || signUp.PasswordHash == string.Empty)
                {
                    response.state = 400;
                    response.message = "Datos incompletos";
                    return BadRequest(response);
                }

                List<User> userList = await GetAllUsers();
                bool existUsername = userList.Where(e => e.Username == signUp.Username).Any();
                bool existEmail = userList.Where(e => e.Email == signUp.Email).Any();
                if (existUsername) 
                {
                    response.state = 400;
                    response.message = $"El username '{signUp.Username}' ya existe";
                    return BadRequest(response); 
                }
                if (existEmail) {
                    response.state = 400;
                    response.message = $"El email '{signUp.Email}' ya existe";
                    return BadRequest(response); 
                }
                var result =  _AuthRepository.Registrer(signUp.Username, signUp.Email, signUp.PasswordHash);
                if (result.Exception != null) 
                {
                    response.state = 400;
                    response.message = result.Exception.Message;
                    return BadRequest(response);
                }
                response.state = 200;
                response.data = "Success";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.state = 500;
                response.message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult>Login(UserLogin Login)
        {
            Response response = new Response();
            try
            {
                if (Login.Username == string.Empty || Login.Password == string.Empty)
                {
                    response.state = 400;
                    response.message = "Datos incompletos";
                    return BadRequest(response);
                }
                List<User> UserList = await GetAllUsers();

                var ValidUser = UserList.FirstOrDefault(user => { return (user.Username == Login.Username) && (user.PasswordHash == Login.Password); });
                if (ValidUser == null)
                {
                    response.state = 400;
                    response.message = "Usuario y/o contraseña Incorrectos";
                    return BadRequest(response);
                }
                response.state = 200;
                response.message = "Success";
                response.data = ValidUser;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.state = 500;
                response.message = ex.Message;
                return BadRequest(response);
            }
        }


        private async Task<List<User>> GetAllUsers() 
        {
            try
            {
                List<User> Users = await _AuthRepository.GetUsersAsync();
                return Users;
            }catch(Exception)
            {
                return new List<User>();
            }
        }
    }
}
