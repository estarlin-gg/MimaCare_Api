using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MimaCare.Infrastructure.Constants;

namespace MimaCare.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            [
            Roles.Admin,
            Roles.Clinic,
            Roles.Doctor
            ];

            foreach (var role in roles)
            {
                var exists =
                    await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role)
                    );
                }
            }
        }
    }
}
