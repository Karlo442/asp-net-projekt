using System;
using System.Linq;
using System.Security.Claims;
using HelperZaOptimalnuKupnju.DTOs;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        /// <summary>
        /// GET /auth/registracija - Prikazuje formu za registraciju
        /// </summary>
        [Route("registracija")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterDTO());
        }

        /// <summary>
        /// POST /auth/registracija - Registrira novog korisnika
        /// </summary>
        [Route("registracija")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Provjera da li email već postoji
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Korisnik sa ovim emailom već postoji");
                    return View(model);
                }

                // Provjera da li korisničko ime već postoji
                var existingUsername = await _userManager.FindByNameAsync(model.UserName);
                if (existingUsername != null)
                {
                    ModelState.AddModelError("UserName", "Korisničko ime je već zauzeto");
                    return View(model);
                }

                var appUser = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    ZipCode = model.ZipCode,
                    EmailConfirmed = true, // Za demo - u produkciji trebalo bi potvrditi email
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                // Dodaj novog korisnika u "Customer" rolu
                await _userManager.AddToRoleAsync(appUser, "Customer");

                // Prijavi korisnika nakon registracije
                await _signInManager.SignInAsync(appUser, isPersistent: false);

                return RedirectToAction("Index", "Home", new { showCelebration = true, type = "registration" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Greška pri registraciji: {ex.Message}");
                return View(model);
            }
        }

        /// <summary>
        /// GET /auth/prijava - Prikazuje formu za prijavu
        /// </summary>
        [Route("prijava")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginDTO());
        }

        /// <summary>
        /// POST /auth/prijava - Prijavljuje korisnika
        /// </summary>
        [Route("prijava")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Provjeri je li to email ili korisničko ime
                AppUser? appUser = null;
                if (model.UserNameOrEmail.Contains("@"))
                {
                    appUser = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
                }
                else
                {
                    appUser = await _userManager.FindByNameAsync(model.UserNameOrEmail);
                }

                if (appUser == null || !appUser.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Korisničko ime/email ili lozinka su pogrešni");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    appUser.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Račun je zaključan. Pokušajte kasnije.");
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWithTwoFactor));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Korisničko ime/email ili lozinka su pogrešni");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Greška pri prijavi: {ex.Message}");
                return View(model);
            }
        }

        [Route("google-prijava")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider = "Google", string? returnUrl = null)
        {
            if (provider == "Google" &&
                (string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"]) ||
                 string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientSecret"])))
            {
                ModelState.AddModelError(string.Empty, "Google prijava nije konfigurirana. Provjeri ClientId i ClientSecret u appsettings.json i restartaj aplikaciju.");
                return View(nameof(Login), new LoginDTO());
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [Route("google-callback")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                ModelState.AddModelError(string.Empty, $"Google prijava nije uspjela: {remoteError}");
                return View(nameof(Login), new LoginDTO());
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Google prijava nije uspjela.");
                return View(nameof(Login), new LoginDTO());
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Google račun nema dostupan email.");
                return View(nameof(Login), new LoginDTO());
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "Korisnik",
                    Address = "-",
                    City = "-",
                    ZipCode = string.Empty,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(nameof(Login), new LoginDTO());
                }
            }

            if (!user.IsActive)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (!logins.Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey))
            {
                await _userManager.AddLoginAsync(user, info);
            }

            if (!await _userManager.IsInRoleAsync(user, "Buyer"))
            {
                await _userManager.AddToRoleAsync(user, "Buyer");
            }

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET /auth/odjava - Odjavljuje korisnika
        /// </summary>
        [Route("odjava")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET /auth/profil - Prikazuje profil trenutnog korisnika
        /// </summary>
        [Route("profil")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserProfileDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                City = user.City,
                ZipCode = user.ZipCode,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };

            return View(model);
        }

        /// <summary>
        /// GET /auth/promjena-lozinke - Prikazuje formu za promjenu lozinke
        /// </summary>
        [Route("promjena-lozinke")]
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDTO());
        }

        /// <summary>
        /// POST /auth/promjena-lozinke - Mijenja lozinku korisnika
        /// </summary>
        [Route("promjena-lozinke")]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    model.OldPassword,
                    model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = "Lozinka je uspješno promijenjena";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Greška pri promjeni lozinke: {ex.Message}");
                return View(model);
            }
        }

        [Route("login-with-two-factor")]
        private IActionResult LoginWithTwoFactor()
        {
            return View();
        }

        /// <summary>
        /// API - POST /auth/api/registracija - Registrira novog korisnika preko API-ja
        /// </summary>
        [Route("api/registracija")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Model nije validan", errors = ModelState });
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "Korisnik sa ovim emailom već postoji" });
                }

                var existingUsername = await _userManager.FindByNameAsync(model.UserName);
                if (existingUsername != null)
                {
                    return BadRequest(new { success = false, message = "Korisničko ime je već zauzeto" });
                }

                var appUser = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    ZipCode = model.ZipCode,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { success = false, message = "Greška pri kreiranju korisnika", errors });
                }

                await _userManager.AddToRoleAsync(appUser, "Customer");

                return Ok(new
                {
                    success = true,
                    message = "Korisnik uspješno registriran",
                    userId = appUser.Id,
                    userName = appUser.UserName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Greška pri registraciji", error = ex.Message });
            }
        }

        /// <summary>
        /// API - POST /auth/api/prijava - Prijavljuje korisnika preko API-ja
        /// </summary>
        [Route("api/prijava")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ApiLogin([FromBody] LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Model nije validan" });
                }

                AppUser? appUser = null;
                if (model.UserNameOrEmail.Contains("@"))
                {
                    appUser = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
                }
                else
                {
                    appUser = await _userManager.FindByNameAsync(model.UserNameOrEmail);
                }

                if (appUser == null || !appUser.IsActive)
                {
                    return Unauthorized(new { success = false, message = "Korisničko ime/email ili lozinka su pogrešni" });
                }

                var result = await _signInManager.PasswordSignInAsync(
                    appUser.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    return Unauthorized(new { success = false, message = "Lozinka je pogrešna" });
                }

                var roles = await _userManager.GetRolesAsync(appUser);

                return Ok(new
                {
                    success = true,
                    message = "Uspješna prijava",
                    userId = appUser.Id,
                    userName = appUser.UserName,
                    email = appUser.Email,
                    firstName = appUser.FirstName,
                    lastName = appUser.LastName,
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Greška pri prijavi", error = ex.Message });
            }
        }

        /// <summary>
        /// API - GET /auth/api/profil - Dohvaća profil trenutnog korisnika preko API-ja
        /// </summary>
        [Route("api/profil")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ApiGetProfile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Korisnik nije pronađen" });
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        address = user.Address,
                        city = user.City,
                        zipCode = user.ZipCode,
                        phoneNumber = user.PhoneNumber,
                        createdAt = user.CreatedAt,
                        isActive = user.IsActive,
                        roles = roles
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Greška pri dohvaćanju profila", error = ex.Message });
            }
        }
    }
}
