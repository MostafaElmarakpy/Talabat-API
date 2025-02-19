using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {

            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Mostafa",
                    Email = "mostafaelmarakpy1@gmail.com",
                    UserName = "Mostafa.Ramadan",
                    PhoneNumber = "0100397361"

                };
                var result = await userManager.CreateAsync(user, "Pa$$w0rd");

                if (result.Succeeded)
                {
                    Console.WriteLine("User created successfully");  // Add this line
                }
                else
                {
                    Console.WriteLine("Failed to create user. Errors:");  // Add this line
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Users already exist in database");  // Add this line
            }
        }

    }
}
