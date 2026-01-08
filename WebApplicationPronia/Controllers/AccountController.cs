using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplicationPronia.Abstractions;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.ViewModels.UserViewModels;

namespace WebApplicationPronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signinManager, RoleManager<IdentityRole> _roleManager, IEmailService _emailService) : Controller
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

            await SendConfirmationMailAsync(newUser);
            TempData["SuccessMessage"] = "Registerden ugurla kecdiniz,zehmet olmasa emailinizi tesdiqleyin!";
            return RedirectToAction("Login");
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
            if (!loginResult)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(loginVM);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email");
                await SendConfirmationMailAsync(user);
                return View(loginVM);
            }
            await _signinManager.SignInAsync(user, loginVM.IsRemember);
            if(!string.IsNullOrWhiteSpace(loginVM.ReturnUrl))
                return Redirect(loginVM.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        private async Task SendConfirmationMailAsync(AppUser appUser)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var url = Url.Action("ConfirmEmail", "Account", new { token = token, userId = appUser.Id }, Request.Scheme);
            string emailBody = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Email Confirmation</title>
</head>
<body style=""margin:0; padding:0; font-family: Arial, Helvetica, sans-serif; background-color:#f4f4f4;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
        <tr>
            <td align=""center"" style=""padding:40px 0;"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:6px; padding:30px;"">
                    <tr>
                        <td align=""center"">
                            <h2 style=""color:#333;"">Confirm Your Email</h2>
                        </td>
                    </tr>
                    <tr>
                        <td style=""color:#555; font-size:15px; line-height:1.6;"">
                            <p>Hello,</p>
                            <p>
                                Thank you for creating an account. Please confirm your email address by clicking the button below.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align=""center"" style=""padding:20px 0;"">
                            <a href=""{url}""
                               style=""background:#007bff; color:#ffffff; text-decoration:none;
                                      padding:12px 24px; border-radius:4px; display:inline-block;"">
                                Confirm Email
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td style=""color:#777; font-size:13px;"">
                            <p>
                                If you did not create this account, you can safely ignore this email.
                            </p>
                            <p>
                                Thank you,<br>
                                <strong>Pronia</strong>
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
            await _emailService.SendEmailAsync(appUser.Email!, "Confirm your email please", emailBody);
        }
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                //await SendConfirmationMailAsync(user);
                return BadRequest();
            }
            await _signinManager.SignInAsync(user, false);
            //return Ok($"Token:{token}, \n UserId:{userId}");
            return RedirectToAction("Index", "Home");
        }
        //public async Task<IActionResult> CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole() 
        //    {
        //        Name="User" 
        //    });
        //    await _roleManager.CreateAsync(new IdentityRole()
        //    {
        //        Name="Admin"
        //    });
        //    await _roleManager.CreateAsync(new IdentityRole()
        //    {
        //        Name = "Moderate"
        //    });
        //    return Ok("Roles Created");
        //}

    }
}
