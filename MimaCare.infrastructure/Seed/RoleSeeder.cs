using Microsoft.AspNetCore.Identity;
using MimaCare.Infrastructure.Constants;

namespace MimaCare.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles =
            [
            Roles.Admin,
            Roles.Clinic,
            Roles.Doctor
            ];

            if (roleManager.Roles.Any())
            {
                return;
            }

            foreach (var role in roles)
            {
                var exists =
                    await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(
                    new IdentityRole<Guid>
                    {
                        Name = role
                    });
                }
            }
        }
    }
}
