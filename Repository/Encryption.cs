using System.Text;
using System.Security.Cryptography;

namespace API.Repository
{
    public class Encryption
    {
        private string _key;
        public Encryption(IConfiguration configuration)
        {
            _key = configuration.GetSection("Routes")["PrivateKey"];
        }
        public async Task<List<string>> DesEncryptRSA(string UsernameEncrypted, string PasswordEncryted)
        {
            try
            {
                List<string> result = new List<string>();
                await Task.Run(()  =>
                {
                    

                    var key = File.ReadAllText(_key);
                    var privateKey = new RSACryptoServiceProvider();
                    privateKey.ImportFromPem(key);

                    byte[] UsernameEncryptedByte = Convert.FromBase64String(UsernameEncrypted);
                    byte[] UsernameDesencryptedByte = privateKey.Decrypt(UsernameEncryptedByte, true);

                    byte[] PasswordEncryptedByte = Convert.FromBase64String(PasswordEncryted);
                    byte[] PasswordDesencryptedByte = privateKey.Decrypt(PasswordEncryptedByte, true);

                    string username = Encoding.UTF8.GetString(UsernameDesencryptedByte);
                    string password = Encoding.UTF8.GetString(PasswordDesencryptedByte);



                    result.Add(username);
                    result.Add(password);
                    return result;
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al desencriptar", ex);
            }
          
        }

        #region METODOS



        #endregion

    }
}
