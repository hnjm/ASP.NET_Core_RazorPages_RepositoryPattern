namespace WebApp.Middlewares
{
    public interface IJwtAuthentication
    {
        string GenerateJwtToken(string username, string role);

        string ValidateToken(string token);
    }
}
