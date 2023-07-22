using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Auth
{
    public class logoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            this.HttpContext.Session.Clear();
            return RedirectToPage("/auth/login");
        }
    }
}
