using C4S.Shared.Utils;
using System.Globalization;

namespace C4S.Services.Services.TokenService
{
    /// <inheritdoc cref="ITokenService" />
    public class TokenService : ITokenService
    {
        private readonly CryptoHelper _cryptoHelper;

        public TokenService(string encryptionKey)
        {
            _cryptoHelper = new CryptoHelper(encryptionKey);
        }

        /// <inheritdoc/>
        public string GenerateToken(Guid userId)
        {
            var creationDate = DateTime.UtcNow;
            var plainText = $"{userId}|{creationDate:O}";
            var result = _cryptoHelper.Encrypt(plainText);
            return result;
        }

        /// <inheritdoc/>
        public (Guid UserId, DateTime Date) DecryptToken(string token)
        {
            var decryptedText = _cryptoHelper.Decrypt(token);
            var parts = decryptedText.Split('|');
            if (parts.Length != 2)
                throw new InvalidOperationException("Invalid token format");

            var userId = Guid.Parse(parts[0]);

            var creationDate = DateTime.Parse(
                parts[1],
                null,
                DateTimeStyles.RoundtripKind);

            return (userId, creationDate);
        }
    }
}