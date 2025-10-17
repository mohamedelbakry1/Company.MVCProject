using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Company.MVCProject.PL.Helpers.Email;
using Company.MVCProject.PL.Helpers.Sms;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Emit;
using System.Threading.Tasks;

namespace Company.MVCProject.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly ITwilioServices _twilioServices;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,IMailService mailService, ITwilioServices twilioServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _twilioServices = twilioServices;
        }

        #region SignUp

        [HttpGet] // GET: Account/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // P@ssw0rd
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user is null)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    if(user is null)
                    {
                        // Register
                        user = new AppUser()
                        {
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            IsAgree = model.IsAgree,
                        };
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            // Send Email to Confirm Email
                            return RedirectToAction("SignIn");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }

                ModelState.AddModelError("", "Invalid SignUp !!");
            }
            return View(model);
        }

        #endregion

        #region SignIn

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    bool flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if(flag)
                    {
                        // Sign In
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                        }
                    }
                }
                ModelState.AddModelError("", "Invalid SignIn !");
            }
            return View(model);
        }

        #endregion

        #region SignOut
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        #endregion

        #region ForgetPassword

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordURL(ForgetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    // Generate Token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Create URL
                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    // Create Email
                    var email = new Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body = url
                    };
                    // Send Email
                    //var flag = EmailSettings.SendEmail(email);
                    var flag = _mailService.SendEmail(email);
                    if (flag)
                    {
                        // Check Your Inbox
                        return RedirectToAction(nameof(CheckYourInbox));
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Reset Password Opertation !!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordSms(ForgetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    // Generate Token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Create URL
                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    // Create Sms
                    var sms = new Sms()
                    {
                        To = user.PhoneNumber,
                        Body = url
                    };
                    // Send Sms
                    await _twilioServices.SendSms(sms);
                    return RedirectToAction(nameof(CheckYourPhone));
                }
            }
            ModelState.AddModelError("", "Invalid Reset Password Opertation !!");
            return View();
        }

        [HttpGet]
        public IActionResult CheckYourInbox()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CheckYourPhone()
        {
            return View();
        }

        #endregion

        #region ResetPassword

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;

                if (email is null || token is null) return BadRequest("Invalid Operations");
                var user = await _userManager.FindByEmailAsync(email);
                if(user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }
                }
                ModelState.AddModelError("", "Invalid Reset Password Operation");
            }
            return View();
        }

        #endregion

        #region Permissions

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region SignIn with Google
        public IActionResult GoogleLogin()
        {
            var prop = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var cliams = result.Principal.Identities.FirstOrDefault().Claims.Select(cliam => new
            {
                cliam.Type,
                cliam.Value,
                cliam.Issuer,
                cliam.OriginalIssuer
            });
            return RedirectToAction("Index", "Home");
        }


        #endregion

        #region SignIn with Facebook
        //public IActionResult FacebookLogin()
        //{
        //    var prop = new AuthenticationProperties()
        //    {
        //        RedirectUri = Url.Action("FacebookResponse")
        //    };
        //    return Challenge(prop, FacebookDefaults.AuthenticationScheme);
        //}

        //public async Task<IActionResult> FacebookResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
        //    var cliams = result.Principal.Identities.FirstOrDefault().Claims.Select(cliam => new
        //    {
        //        cliam.Type,
        //        cliam.Value,
        //        cliam.Issuer,
        //        cliam.OriginalIssuer
        //    });
        //    return RedirectToAction("Index", "Home");
        //}


        #endregion

    }
}
