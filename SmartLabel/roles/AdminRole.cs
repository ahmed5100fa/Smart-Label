using Microsoft.AspNetCore.Identity;

namespace SmartLabel.roles
{
    public class AdminRole
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
          
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

           
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

          
        }
    }
}
