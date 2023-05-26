using System;
using System.IO;
using System.Security.Cryptography;

namespace BnPBank.Services
{
    public class SessionTokenManager
    {
        private readonly string sessionTokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BnPBank", "sessionToken.dat");
        public string AuthenticatedUsername { get; private set; }

        public void StoreToken(string token, string username)
        {
            byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);

            byte[] encryptedTokenBytes = ProtectedData.Protect(tokenBytes, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(sessionTokenFilePath, encryptedTokenBytes);
            AuthenticatedUsername = username;
        }

        public string RetrieveToken()
        {
            if (!File.Exists(sessionTokenFilePath))
            {
                return null;
            }

            byte[] encryptedTokenBytes = File.ReadAllBytes(sessionTokenFilePath);

            byte[] tokenBytes = ProtectedData.Unprotect(encryptedTokenBytes, null, DataProtectionScope.CurrentUser);

            return System.Text.Encoding.UTF8.GetString(tokenBytes);
        }

        public void ClearToken()
        {
            if (File.Exists(sessionTokenFilePath))
            {
                File.Delete(sessionTokenFilePath);
                AuthenticatedUsername = null;
            }
        }
    }
}