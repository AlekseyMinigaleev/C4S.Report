using System.Security.Cryptography;
using System.Text;

namespace C4S.Shared.Utils
{
    /// <summary>
    /// Вспомогательный класс для шифрования и дешифрования текстовых данных с использованием алгоритма AES.
    /// </summary>
    public class CryptoHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CryptoHelper"/> с заданным ключом.
        /// </summary>
        /// <param name="key">Ключ шифрования в виде строки.</param>
        public CryptoHelper(string key)
        {
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            _iv = new byte[16];
        }

        /// <summary>
        /// Шифрует заданный текст с использованием алгоритма AES.
        /// </summary>
        /// <param name="plainText">Текст, который нужно зашифровать.</param>
        /// <returns>Зашифрованный текст в виде строки Base64.</returns>
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                sw.Write(plainText);
            }

            var result = Convert.ToBase64String(ms.ToArray());
            return result;
        }

        /// <summary>
        /// Шифрует заданный текст с использованием алгоритма AES.
        /// </summary>
        /// <param name="plainText">Текст, который нужно зашифровать.</param>
        /// <returns>Зашифрованный текст в виде строки Base64.</returns>
        public string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var result = sr.ReadToEnd();
            return result;
        }
    }
}