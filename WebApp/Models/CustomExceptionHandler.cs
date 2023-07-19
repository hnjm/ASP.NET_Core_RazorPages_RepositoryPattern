namespace WebApp.Models
{
    public class CustomExceptionHandler
    {
        
        public CustomExceptionHandler()
        {
            
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var feature = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            var error = feature?.Error;

            if (error != null)
            {
              //  LogError(error);
                throw error;
            }

            await Task.CompletedTask;
        }
    }
}
