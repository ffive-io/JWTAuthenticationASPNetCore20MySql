using FFive.API.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace FFive.API.AuthPolicies
{
    public class MinimumAgeHandler : AuthorizationHandler<AgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequirement requirement)
        {
            var user = context.User;
            if (!user.HasClaim(c => c.Type == Extensions.AgeClaim))
                return Task.CompletedTask;

            var userAge = int.Parse(user.FindFirst(Extensions.AgeClaim).Value);
            if (userAge >= requirement.Age)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}