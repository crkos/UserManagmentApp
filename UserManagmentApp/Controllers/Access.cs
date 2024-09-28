using Microsoft.AspNetCore.Mvc;
using UserManagmentApp.Data;
using Microsoft.EntityFrameworkCore;
using UserManagmentApp.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UserManagmentApp.Models;

namespace UserManagmentApp.Controllers
{
    public class Access : Controller
    {
        private readonly AppDBContext _dbContext;

        public Access(AppDBContext AppDbContext ) 
        {
            _dbContext = AppDbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserVM model)
        {
            // Could implement other types of validation such as email validation on here,etc , requirements arent clear so i assume out of scope.
            // This is done because requirement needs at least 1 character password, so we reject empty passwords.

            // "Frontend" is validated but we know that, is just for UI users and other people could use directly the forms and entry invalid data.
            if (model.Password.Length == 0 || model.ConfirmPassword.Length == 0)
            {
                ViewData["Message"] = "Password should not be empty, Please enter at least 1 character password.";
                return View();
            }


            if (model.Password != model.ConfirmPassword)
            {
                ViewData["Message"] = "Passwords do not match.";
                return View();
            }

            User user = new User()
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            };

            try
            {
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                // Could determine or send an array of errors but this is just simplified...
                ViewData["Message"] = "This email is already registered.";
                return View();
            }

            if (user.Id != 0) return RedirectToAction("Login", "Access");

            ViewData["Message"] = "Something unexpected happened, try again later.";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            User? user = await _dbContext.Users
                                  .Where(u =>
                                      u.Email == model.Email
                                  ).FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["Message"] = "The email/password is incorrect";
                return View();
            }

            if (user.Status == "Blocked")
            {
                ViewData["Message"] = "This user is currently blocked, contact an administrator to unblock you.";
                return View();
            }

            bool validatedPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

            if (!validatedPassword)
            {
                ViewData["Message"] = "The email/password is incorrect";
                return View();
            }

            user.LastLogin = DateTime.Now;

            try
            {
                _dbContext.SaveChanges();
            } catch (DbUpdateException ex)
            {
                ViewData["Message"] = "Something unexpected happened, try again later.";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
                );

            return RedirectToAction("Index", "Home");

        }
    }
}
