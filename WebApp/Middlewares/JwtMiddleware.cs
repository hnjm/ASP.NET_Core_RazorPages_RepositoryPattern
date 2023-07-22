using WebApp.Models;

namespace WebApp.Middlewares
{
	public class JwtMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly IConfiguration configuration;
        private readonly IUserInfoService userService;
        private readonly IJwtAuthentication jwtAuthentication;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IUserInfoService userService, IJwtAuthentication authentication)
        {
            this._next = next;
            this.configuration = configuration;
            this.userService = userService;
            this.jwtAuthentication = authentication;
        }

        public async Task Invoke(HttpContext context)
        {
            string jwTSessionToken = context.Session.GetString("jwt");

            if (!string.IsNullOrEmpty(jwTSessionToken))
            {
                // if we set the Authorization header, JWT configuration inprogram.cs will validate because of UseAuthentication Method
                // This will populate the Claims in UserContext in HttpContext.
                context.Request.Headers.Add("Authorization", "Bearer " + jwTSessionToken);

                // Additionally we are setting the user info in Items after validate in system.
                setUserInfoToContext(context, jwTSessionToken);
            }                          

            await _next(context);
        }

        public void setUserInfoToContext(HttpContext context, string token)
        {
            try
            {
                var username = this.jwtAuthentication.ValidateToken(token);
                var userdata = userService.GetUserInfo(username);
                context.Items["userToken"] = new { Username = userdata.UserName, Role = userdata.Role };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
