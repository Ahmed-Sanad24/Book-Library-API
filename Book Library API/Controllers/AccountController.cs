using Bill_system_API.DTOs;
using Bill_system_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bill_system_API.Controllers
{
    /// <summary>
    /// Handles user account-related actions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">The details of the user to register.</param>
        /// <returns>A status message indicating the result of the registration process.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(R_NewUserDTO user)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = await _userManager.FindByEmailAsync(user.email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already exists");
                    return BadRequest(ModelState);
                }
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = user.userName,
                    Email = user.email,
                    PhoneNumber = user.phoneNumber
                };
                // Check if there are any existing users
                var anyUsers = _userManager.Users.Any();
                IdentityResult result = await _userManager.CreateAsync(applicationUser, user.password);
                if (result.Succeeded)
                {
                    string role = anyUsers ? "User" : "Admin";

                    // Assign role to user
                    await _userManager.AddToRoleAsync(applicationUser, role);
                    return Ok("Succeeded Registration");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="login">The login details.</param>
        /// <returns>A JWT token if login is successful.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, login.Password))
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);

                        List<Claim> userData = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        };

                        foreach (var role in userRoles)
                        {
                            userData.Add(new Claim(ClaimTypes.Role, role));
                        }

                        string key = "Welcome to my secret key in Bill System";
                        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
                        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            claims: userData,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: signingCredentials
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                        return Ok(new { token = tokenString });
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email is Invalid");
                }
            }
            return BadRequest(ModelState);
        }
    }
}

