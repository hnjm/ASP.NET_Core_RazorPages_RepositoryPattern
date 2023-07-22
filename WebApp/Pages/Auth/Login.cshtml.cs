using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Middlewares;
using WebApp.Models;

namespace WebApp.Pages.Auth
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IJwtAuthentication _jwtAuthentication;

        [BindProperty]
        public UserInfo UserInfoData { get; set; }

        public LoginModel(IUserInfoService userInfoService, IJwtAuthentication jwtAuthentication)
        {
            _userInfoService = userInfoService;
            _jwtAuthentication = jwtAuthentication;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostLoginUser()
        {
            var data = this._userInfoService.GetUserInfo(UserInfoData.UserName);

            if(data == null || data.Password != UserInfoData.Password)
            {
                return RedirectToPage("/Index");
            }

            var token = this._jwtAuthentication.GenerateJwtToken(data.UserName, data.Role);

            this.HttpContext.Session.SetString("jwt", token);

           return RedirectToPage("/Author/Index");
        }
    }
}
