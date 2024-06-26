﻿using Humanizer;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SoftwareTracker.Data
{
    public class EncryptionHelper
    {
        

        public static string Encrypt(string plainText)
        {
            string EncryptionKey = AkeylessHelper.RetrieveSecret("EncryptionKey");
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // Use a random IV for each encryption for more security

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = AkeylessHelper.RetrieveSecret("EncryptionKey");
            byte[] decryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // Use the same IV as used for encryption

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

}
