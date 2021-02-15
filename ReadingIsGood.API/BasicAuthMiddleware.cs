using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReadingIsGood.API.Models;

namespace ReadingIsGood.API
{
     /// <summary>
    /// Defines the <see cref="BasicAuthMiddleware" />.
    /// </summary>
    public class BasicAuthMiddleware
    {
        #region Fields

        /// <summary>
        /// Defines the next.
        /// </summary>
        private readonly RequestDelegate next;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next<see cref="RequestDelegate"/>.</param>
        public BasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        //public BasicAuthMiddleware(RequestDelegate next)
        //{
        //    this.next = next;
        //}

        #endregion

        #region Methods

        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <param name="context">The context<see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await next(context).ConfigureAwait(false);
                return;
            }

            if (!authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await next(context).ConfigureAwait(false);
                return;
            }


            var token = authHeader.Substring(6).Trim();
            var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialString.Split(':');

            var username = credentials[0]?.Trim();
            var password = credentials[1]?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await next(context).ConfigureAwait(false);
                return;
            }


            //TODO:Kudret-->you have to change for production, it's just for development
            if (username?.Trim() != "kudret" && password?.Trim() != "kurt")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await next(context).ConfigureAwait(false);
                return;
            }

            if (context.Response.StatusCode != StatusCodes.Status200OK)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            var deserializeModel = new UserModel()
            {
                CustomerId = Guid.Parse("4833489d-1e39-42e4-b71e-94b47815d052"),
                CustomerName = "PowerCompany",
                UserId = Guid.Parse("60ef2ab9-6f46-44e0-8b1b-ecbaac53f837"),
                UserName = "kudretkurt"
            };

            var claims = new[]
            {
                new Claim("UserId", deserializeModel.UserId.ToString()),
                new Claim("CustomerName", deserializeModel.CustomerName),
                new Claim("CustomerId", deserializeModel.CustomerId.ToString()),
                new Claim("UserName", deserializeModel.UserName)
            };
            var identity = new ClaimsIdentity(claims, "Basic");
            context.User = new ClaimsPrincipal(identity);


            await next(context).ConfigureAwait(false);
        }

        #endregion
    }
}