using System;
using System.Security.Cryptography;
using System.Text;

namespace CERS

{
    public class AESCryptography
    {
        public static string EncryptAES(string plainText)
        {
            try
            {
                string key = Preferences.Get("EncKey", "xxxxxxxxxxxxxxxx"); ;
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encrypt(plainBytes, getAesManaged(key)));
            }
            catch (Exception)
            {
                return  "";
            }
        }
        public static string DecryptAES(string encryptedText)
        {
            try
            {
                string key = Preferences.Get("EncKey", "xxxxxxxxxxxxxxxx"); ;
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getAesManaged(key)));
            }
            catch (Exception)
            {
                return  "";
            }
        }
        private static Aes getAesManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = keyBytes;
            aes.IV = keyBytes;
            return aes;
        }
        private static byte[] Encrypt(byte[] plainBytes, Aes aes)
        {
            return aes.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
        private static byte[] Decrypt(byte[] encryptedData, Aes aes)
        {
            return aes.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}