// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Melodix.Models;
using Melodix.Models.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Melodix.MVC.Areas.Identity.Pages.Account
{
    public class RegisterModel:PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "El campo Nick es obligatorio.")]
            public string Nick { get; set; }

            [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
            public string Nombre { get; set; }

            [Display(Name = "Fecha de nacimiento")]
            [DataType(DataType.Date)]
            public DateTime? FechaNacimiento { get; set; }

            [Display(Name = "Género")]
            public GeneroUsuario? Genero { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if(ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Asigna propiedades personalizadas
                user.Nick = Input.Nick;
                user.Nombre = Input.Nombre;

                // Asegura que la fecha de nacimiento sea UTC para Postgres
                if(Input.FechaNacimiento.HasValue)
                {
                    var fecha = Input.FechaNacimiento.Value;
                    if(fecha.Kind == DateTimeKind.Unspecified)
                        user.FechaNacimiento = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
                    else
                        user.FechaNacimiento = fecha.ToUniversalTime();
                }
                else
                {
                    user.FechaNacimiento = null;
                }

                user.Genero = Input.Genero;
                user.Activo = true;
                user.CreadoEn = DateTime.UtcNow;
                user.ActualizadoEn = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if(result.Succeeded)
                {
                    // Verifica que el rol "users" exista, si no, lo crea
                    if(!await _userManager.IsInRoleAsync(user, "users"))
                    {
                        var roleManager = HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
                        if(roleManager != null && !await roleManager.RoleExistsAsync("users"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("users"));
                        }
                    }

                    // Asigna el rol "users" y verifica el resultado
                    var addRoleResult = await _userManager.AddToRoleAsync(user, "users");
                    if(!addRoleResult.Succeeded)
                    {
                        foreach(var error in addRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, $"Error al asignar rol: {error.Description}");
                        }
                        // Si falla, muestra el formulario con los errores
                        return Page();
                    }

                    _logger.LogInformation("User created a new account with password and assigned to 'users' role.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if(_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                                                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if(!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
    }