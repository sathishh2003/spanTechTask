using System;
using System.Text;

namespace SpanTechTask.Services
{
    public class Base64EncryptionService
    {
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Decrypt(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
                throw new ArgumentNullException(nameof(base64EncodedData));

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
