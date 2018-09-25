using FFive.API.Models;
using FFive.API.Providers;
using FFive.API.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FFive.API.Services
{
    public class TokenService
    {
        private readonly KookifyDbContext _kookifyDbContext;
        private readonly ILogger<TokenService> _logger;

        public TokenService(KookifyDbContext kookifyDbContext, ILogger<TokenService> logger)
        {
            _kookifyDbContext = kookifyDbContext;
            _logger = logger;
        }

        private void Insert(RefreshToken token)
        {
            try
            {
                var tokenModel = _kookifyDbContext.RefreshTokens.SingleOrDefault(i => i.UserId == token.UserId);
                if (tokenModel != null)
                {
                    _kookifyDbContext.RefreshTokens.Remove(tokenModel);
                    _kookifyDbContext.SaveChanges();
                }
                _kookifyDbContext.RefreshTokens.Add(token);
                _kookifyDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public TokenProviderOptions GetOptions()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.Config.GetSection("TokenAuthentication:SecretKey").Value));

            return new TokenProviderOptions
            {
                Path = Configuration.Config.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.Config.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.Config.GetSection("TokenAuthentication:Issuer").Value,
                Expiration = TimeSpan.FromMinutes(Convert.ToInt32(Configuration.Config.GetSection("TokenAuthentication:ExpirationMinutes").Value)),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
        }

        public LoginResponseDTO GetToken(ApplicationUser user, RefreshToken refreshToken = null)
        {
            var options = GetOptions();
            var now = DateTime.UtcNow;

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            };

            var userClaims = _kookifyDbContext.UserClaims.Where(i => i.UserId == user.Id);
            foreach (var userClaim in userClaims)
            {
                claims.Add(new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            }
            var userRoles = _kookifyDbContext.UserRoles.Where(i => i.UserId == user.Id);
            foreach (var userRole in userRoles)
            {
                var role = _kookifyDbContext.Roles.Single(i => i.Id == userRole.RoleId);
                claims.Add(new Claim(Extensions.RoleClaimType, role.Name));
            }

            if (refreshToken == null)
            {
                refreshToken = new RefreshToken()
                {
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString("N"),
                };
                Insert(refreshToken);
            }

            refreshToken.IssuedUtc = now;
            refreshToken.ExpiresUtc = now.Add(options.Expiration);
            _kookifyDbContext.SaveChanges();

            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims.ToArray(),
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new LoginResponseDTO
            {
                access_token = encodedJwt,
                refresh_token = refreshToken.Token,
                expires_in = (int)options.Expiration.TotalSeconds,
                user_name = user.UserName,
                first_name = user.FirstName,
                last_name = user.LastName,
                is_admin = claims.Any(i => i.Type == Extensions.RoleClaimType && i.Value == Extensions.AdminRole)
            };
            return response;
        }
    }
}