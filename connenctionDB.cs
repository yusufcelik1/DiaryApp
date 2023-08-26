using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Odev2
{
    public class connenctionDB
    {
        static byte[] key = new byte[32]; // 256-bit key
        static byte[] iv = new byte[16]; // 128-bit Initialization Vector
        public static SqlConnection connection;
        public static string connectionString = @"Server=DESKTOP-FLOJOJC\SQLEXPRESS; Database=dailyApp; Trusted_Connection=SSPI; MultipleActiveResultSets=true; TrustServerCertificate=true;";

        public static bool IsDateValid(string input)
        {
            // Check if the input consists of only digits and dots
            foreach (char c in input)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    return false;
                }
            }

            // Split the input by dots
            string[] parts = input.Split('.');

            // Check if there are three parts
            if (parts.Length != 3)
            {
                return false;
            }

            // Check if each part is a valid integer
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out _))
                {
                    return false;
                }
            }

            return true;
        }

        public static string EncryptContent(string content)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(content);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptContent(string encryptedContent)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedContent)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
