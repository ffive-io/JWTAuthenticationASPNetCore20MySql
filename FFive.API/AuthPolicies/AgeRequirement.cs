using Microsoft.AspNetCore.Authorization;

namespace FFive.API.AuthPolicies
{
    public class AgeRequirement : IAuthorizationRequirement
    {
        public int Age { get; private set; }

        public AgeRequirement(int minAge)
        {
            Age = minAge;
        }
    }
}