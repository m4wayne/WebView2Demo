using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebSimulator.Controllers
{
	public class AuthController : Controller
	{
		public IActionResult Index(IFormCollection collection)
		{
            string username = collection["username"].ToString();
            string password = collection["password"].ToString();

            if ("demo".Equals(username) && "demo".Equals(password))
            {
                return new RedirectResult("~/Home");
            }
            else
            {
                ViewBag.ErrorTip = "Invalid username or password.";
                return View("/Pages/Index.cshtml");
            }
        }
    }
}
