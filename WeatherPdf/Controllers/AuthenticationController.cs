using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherPdf.Models.Dtos;
using WeatherPdf.Models.ResponseModels;
using WeatherPdf.Services.Email;
using WeatherPdf.Services.Security;

namespace WeatherPdf.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TokenGenerator _token;
    private readonly IEmailService _email;
    public AuthenticationController(UserManager<IdentityUser> userManager, TokenGenerator token, IEmailService email)
    {
        _userManager = userManager;
        _email = email;
        _token = token;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
    {
        if (ModelState.IsValid)
        {
            var isUserExist = await _userManager.FindByEmailAsync(requestDto.Email);
            if (isUserExist is not null)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already exist"
                    }
                });
            }

            var newUser = new IdentityUser()
            {
                UserName = requestDto.Name,
                Email = requestDto.Email
            };

            var isUserCreated = await _userManager.CreateAsync(newUser, requestDto.Password);

            if (isUserCreated.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var emailNotification = await _email.SendEmail(new EmailMetadata
                {
                    ToAddress = requestDto.Email,
                    Subject = "Email confirmation letter",
                    Body = $"{code}"
                });
                if (emailNotification)
                {
                    return Ok("check email");
                }
                return Ok(new { message = $"Please confirm your email with the code that you have received {code}" });
            }

            var errors = isUserCreated.Errors.Select(x => x.Description).ToList();
            return BadRequest(new AuthResult()
            {
                Errors = errors,
                Result = false
            });
        }
        return BadRequest();
    }


    [Route("Login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
    {
        if (ModelState.IsValid)
        {
            var isUserExisted = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (isUserExisted is null)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    },
                    Result = false
                });
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(isUserExisted);
            if (!isEmailConfirmed)
            {
                return Unauthorized(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                        "Email is not confirmed"
                    }
                });
            }

            if (await _userManager.IsLockedOutAsync(isUserExisted))
            {
                return Unauthorized(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                        "User is locked out"
                    }
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(isUserExisted, loginRequest.Password);
            if (!isCorrect)
            {
                await _userManager.AccessFailedAsync(isUserExisted);

                if (await _userManager.IsLockedOutAsync(isUserExisted))
                {
                    return Unauthorized(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "User is lockedout please reset your password"
                        },
                        Result = false
                    });
                }
                else
                {
                    return Unauthorized(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid credentials"
                        },
                        Result = false
                    });
                }
            }
            await _userManager.ResetAccessFailedCountAsync(isUserExisted);

            var jwtToken = _token.GenerateToken(loginRequest.Email);

            return Ok(new AuthResult()
            {
                Token = jwtToken,
                Result = true
            });

        }

        return BadRequest(new AuthResult()
        {
            Errors = new List<string>()
            {
                "Invalid payload"
            },
            Result = false
        });
    }



    [HttpPost]
    [Route("EmailVerification")]
    public async Task<IActionResult> EmailVerification(string? email, string? code)
    {
        if (email == null || code == null)
            return BadRequest("Invalid payload");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest("Invalid payload");

        var isVerified = await _userManager.ConfirmEmailAsync(user, code);
        if (isVerified.Succeeded)
            return Ok(new
            {
                message = "email confirmed"
            });

        return BadRequest("something went wrong");
    }
}
