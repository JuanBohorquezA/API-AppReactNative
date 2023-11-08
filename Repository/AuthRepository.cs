using API.Models;
using System.Data;
using System.Data.SqlClient;

namespace API.Repository
{
    public class AuthRepository
    {
        public readonly string con;

        public AuthRepository(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("Connection");

        }
        public async Task Registrer(string Username, string Email, string PasswordHash)
        {
            using(SqlConnection connection = new (con))
            {
                await connection.OpenAsync();
                using(SqlCommand cmd = new("CreateUser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", Username);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine(cmd);
                }

            }
        }
        public async Task<List<User>> GetUsersAsync()
        {   
            List<User> UsersList = new List<User>();
            using (SqlConnection connection = new(con))
            {
               await connection.OpenAsync();
                using (SqlCommand cmd = new("GetAllUsers", connection)) 
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            User User = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = Convert.ToString(reader["Username"]),
                                Email = Convert.ToString(reader["Email"]),
                                PasswordHash = Convert.ToString(reader["PasswordHash"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };
                            UsersList.Add(User);
                        }
                    }
                }
            }
            return UsersList;

        }
    }
}
