using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagmentApp.Data;
using UserManagmentApp.Models;
using UserManagmentApp.NewFolder;
using Azure.Core;


namespace UserManagmentApp.Controllers
{
    [Authorize]
    public class UserChange : Controller
    {
        private readonly AppDBContext _dbContext;

        public UserChange(AppDBContext AppDbContext)
        {
            _dbContext = AppDbContext;
        }

        [HttpPatch]
        public async Task<IActionResult> SetStatus([FromBody] UserStatusUpdateRequest request)
        {
            if(request.Status != "Blocked" && request.Status != "Active")
            {
                return Json(new { success = false, message = "The actions you want to do are invalid, only active or blocked are valid" });
            }


            if (request.UserIds == null || request.UserIds.Count == 0)
            {
                return Json(new { success = false, message = "No users selected" });
            }

            var usersToUpdate = _dbContext.Users.Where(u => request.UserIds.Contains(u.Id)).ToList();
            foreach (var user in usersToUpdate)
            {
                user.Status = request.Status;
            }

            _dbContext.SaveChanges();

            var currentUser = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            var isUserBlocked = usersToUpdate.Find(user => user.Email == currentUser);

            if (isUserBlocked != null && request.Status == "Blocked")
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            } 

            return Json(new { success = true, message = $"Users are now {request.Status.ToLower()}" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return Json(new { success = false, message = "No users selected" });
            }

            var usersToDelete = _dbContext.Users.Where(u => userIds.Contains(u.Id)).ToList();
            _dbContext.Users.RemoveRange(usersToDelete);
            _dbContext.SaveChanges();

            var currentUser = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            var isDeletedUser = usersToDelete.Find(user => user.Email == currentUser);

            if (isDeletedUser != null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return Json(new { success = true, message = "Users have been deleted" });
        }
    }
}
