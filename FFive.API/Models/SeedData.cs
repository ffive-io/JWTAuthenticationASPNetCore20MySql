using FFive.API.Utils;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFive.API.Models
{
    public class SeedData
    {
        public SeedData(UserManager<ApplicationUser> userManager)
        {
        }

        public SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
        }

        public static async Task Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAndClaims(userManager, roleManager);
            await SeedAdmin(userManager);
            await SeedUser25(userManager);
            await SeedUser16(userManager);
        }

        private static async Task SeedRolesAndClaims(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Extensions.AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = Extensions.AdminRole
                });
            }

            if (!await roleManager.RoleExistsAsync(Extensions.UserRole))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = Extensions.UserRole
                });
            }

            var adminRole = await roleManager.FindByNameAsync(Extensions.AdminRole);
            var adminRoleClaims = await roleManager.GetClaimsAsync(adminRole);

            if (!adminRoleClaims.Any(x => x.Type == Extensions.ManageUserClaim))
            {
                await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim(Extensions.ManageUserClaim, "true"));
            }
            if (!adminRoleClaims.Any(x => x.Type == Extensions.AdminClaim))
            {
                await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim(Extensions.AdminClaim, "true"));
            }

            var userRole = await roleManager.FindByNameAsync(Extensions.UserRole);
            var userRoleClaims = await roleManager.GetClaimsAsync(userRole);
            if (!userRoleClaims.Any(x => x.Type == Extensions.UserClaim))
            {
                await roleManager.AddClaimAsync(userRole, new System.Security.Claims.Claim(Extensions.UserClaim, "true"));
            }
        }

        private static async Task SeedAdmin(UserManager<ApplicationUser> userManager)
        {
            var u = await userManager.FindByNameAsync("admin@kookify.com");
            if (u == null)
            {
                u = new ApplicationUser
                {
                    UserName = "admin@kookify.com",
                    Email = "admin@kookify.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    FirstName = "admin",
                    LastName = "user"
                };
                var x = await userManager.CreateAsync(u, "Admin1234!");
            }
            var uc = await userManager.GetClaimsAsync(u);
            if (!uc.Any(x => x.Type == Extensions.AdminClaim))
            {
                await userManager.AddClaimAsync(u, new System.Security.Claims.Claim(Extensions.AdminClaim, true.ToString()));
            }
            if (!await userManager.IsInRoleAsync(u, Extensions.AdminRole))
                await userManager.AddToRoleAsync(u, Extensions.AdminRole);
        }

        private static async Task SeedUser25(UserManager<ApplicationUser> userManager)
        {
            var u = await userManager.FindByNameAsync("user25@kookify.com");
            if (u == null)
            {
                u = new ApplicationUser
                {
                    UserName = "user25@kookify.com",
                    Email = "user25@kookify.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    FirstName = "normal",
                    LastName = "user"
                };
                var x = await userManager.CreateAsync(u, "Normal1234!");
            }
            var uc = await userManager.GetClaimsAsync(u);
            if (!uc.Any(x => x.Type == Extensions.UserClaim))
            {
                await userManager.AddClaimAsync(u, new System.Security.Claims.Claim(Extensions.UserClaim, true.ToString()));
            }
            if (!uc.Any(x => x.Type == Extensions.AgeClaim))
            {
                await userManager.AddClaimAsync(u, new System.Security.Claims.Claim(Extensions.AgeClaim, "25"));
            }

            if (!await userManager.IsInRoleAsync(u, Extensions.UserRole))
                await userManager.AddToRoleAsync(u, Extensions.UserRole);
        }

        private static async Task SeedUser16(UserManager<ApplicationUser> userManager)
        {
            var u = await userManager.FindByNameAsync("user16@kookify.com");
            if (u == null)
            {
                u = new ApplicationUser
                {
                    UserName = "user16@kookify.com",
                    Email = "user16@kookify.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    FirstName = "normal",
                    LastName = "user"
                };
                var x = await userManager.CreateAsync(u, "Normal1234!");
            }
            var uc = await userManager.GetClaimsAsync(u);
            if (!uc.Any(x => x.Type == Extensions.UserClaim))
            {
                await userManager.AddClaimAsync(u, new System.Security.Claims.Claim(Extensions.UserClaim, true.ToString()));
            }
            if (!uc.Any(x => x.Type == Extensions.AgeClaim))
            {
                await userManager.AddClaimAsync(u, new System.Security.Claims.Claim(Extensions.AgeClaim, "16"));
            }

            if (!await userManager.IsInRoleAsync(u, Extensions.UserRole))
                await userManager.AddToRoleAsync(u, Extensions.UserRole);
        }
    }
}