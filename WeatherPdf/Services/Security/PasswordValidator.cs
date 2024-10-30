using Microsoft.AspNetCore.Identity;

namespace WeatherPdf.Services.Security;

public class PasswordValidator<T> : IPasswordValidator<T> where T : class
{
    public async Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user, string? password)
    {
        var email = await manager.GetUserNameAsync(user);

        if (string.Equals(email, password, StringComparison.Ordinal))
            return IdentityResult.Failed(new IdentityError()
            {
                Description = "Email cannot be used in password",
                Code = "Duplication"
            });

        return IdentityResult.Success;
    }
}
