using BookingMvc.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(
            UserManager<IdentityUser> userMgr,
            SignInManager<IdentityUser> signInMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            roleManager = roleMgr;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated == true)
                return Redirect(returnUrl);

            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                        return Redirect(model.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Невірний email або пароль");
            }
            return View(model);
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Enforce unique email
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Користувач з таким email вже існує");
                    return View(model);
                }

                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign role (User or Admin)
                    string role = model.Role == "Admin" ? "Admin" : "User";
                    await userManager.AddToRoleAsync(user, role);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET/POST: /Account/Logout
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            var model = new UserProfileModel
            {
                Email = user.Email ?? "",
                UserName = user.UserName
            };
            return View(model);
        }

        // POST: /Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            if (!ModelState.IsValid)
                return View(model);

            // Update email
            if (user.Email != model.Email)
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Цей email вже використовується");
                    return View(model);
                }
                user.Email = model.Email;
                user.UserName = model.Email;
                await userManager.UpdateAsync(user);
            }

            // Change password if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Введіть поточний пароль");
                    return View(model);
                }
                var passwordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
                await signInManager.RefreshSignInAsync(user);
            }

            TempData["message"] = "Профіль успішно оновлено";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
