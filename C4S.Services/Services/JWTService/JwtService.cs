using C4S.DB.Models;
using C4S.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace C4S.Services.Services.JWTService
{
    /// <summary>
    /// <inheritdoc cref="IJwtService"/>
    /// </summary>
    public class JwtServise : IJwtService
    {
        public JwtServise(JwtConfiguration jwtConfig)
        {
            JwtConfig = jwtConfig;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public JwtConfiguration JwtConfig { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime AccessTokenExpiry =>
            DateTime.Now.Add(TimeSpan.FromMinutes(JwtConfig.Expiries.AccessTokenExpiryInMinutes));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime RefreshTokenExpiry =>
             DateTime.Now.Add(TimeSpan.FromDays(JwtConfig.Expiries.RefreshTokenExpiryInDays));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecurityKey));

        private IEnumerable<Claim>? _claims { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string CreateJwtToken(UserModel user, DateTime expiry)
        {
            if (_claims is null)
                CreateAndSetClaims(user);

            var jwt = new JwtSecurityToken(
                    issuer: JwtConfig.Issuer,
                    audience: JwtConfig.Audience,
                    expires: expiry,
                    signingCredentials: new SigningCredentials(
                        SymmetricSecurityKey,
                        SecurityAlgorithms.HmacSha256),
                    claims: _claims);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private IEnumerable<Claim> CreateAndSetClaims(UserModel user)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };

            if (user.RsyaAuthorizationToken is not null)
                claims.Add(
                    new Claim(nameof(user.RsyaAuthorizationToken),
                    user.RsyaAuthorizationToken));

            _claims = claims;

            return claims;
        }
    }
}