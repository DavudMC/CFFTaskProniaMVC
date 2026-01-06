using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.ViewModels.UserViewModels;

namespace WebApplicationPronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signinManager,RoleManager<IdentityRole> _roleManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) 
            {
                return View(registerVM);
            }
            var existUserName = await _userManager.FindByNameAsync(registerVM.UserName);
            if (existUserName != null) 
            {
                ModelState.AddModelError("UserName", "This User already exists");
                return View(registerVM);
            }
            var existEmailAddress = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if (existEmailAddress is { }) 
            {
                ModelState.AddModelError(nameof(registerVM.EmailAddress), "This email address is already exists");
                return View(registerVM);
            }
            AppUser newUser = new() 
            {
                FullName = registerVM.FirstName + " " + registerVM.LastName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.UserName
            };
            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!result.Succeeded) 
            {
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(registerVM); 
                }
            }
            await _signinManager.SignInAsync(newUser, false);
            return RedirectToAction("Index","Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) 
            {
                return View(loginVM);
            }   
            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user == null) 
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(loginVM);
            }
            var loginResult = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if(!loginResult)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(loginVM);
            }
            await _signinManager.SignInAsync(user, loginVM.IsRemember);
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole() 
            {
                Name="User" 
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name="Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderate"
            });
            return Ok("Roles Created");
        }
    }
}
