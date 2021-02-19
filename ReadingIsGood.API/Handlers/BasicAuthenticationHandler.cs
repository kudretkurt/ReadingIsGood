using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadingIsGood.API.Models;

namespace ReadingIsGood.API.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] {':'}, 2);
                var username = credentials[0];
                var password = credentials[1];

                //TODO:Kudret-->you have to change for production, it's just for development
                if (!(username?.Trim() == "kudret" && password?.Trim() == "kurt"))
                    return AuthenticateResult.Fail("Invalid Username or Password");
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var user = new UserModel
            {
                CustomerId = Guid.Parse("4833489d-1e39-42e4-b71e-94b47815d052"),
                CustomerName = "PowerCompany",
                UserId = Guid.Parse("60ef2ab9-6f46-44e0-8b1b-ecbaac53f837"),
                UserName = "kudretkurt"
            };

            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("CustomerName", user.CustomerName),
                new Claim("CustomerId", user.CustomerId.ToString()),
                new Claim("UserName", user.UserName)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}