namespace MinimalAPI.Auth;

public class AuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var userData = Jwt.GetUserData(context.User.Claims);

        if (userData is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "No or wrong authorization token provided"));
        }

        return Task.CompletedTask;
    }
}