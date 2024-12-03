using DAW.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAW.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Roles.Any())
                {
                    return;
                }
                context.Roles.AddRange(
                    new IdentityRole
                    {
                        Id = "c62037fa-90d9-4fa7-8a27-e1e4cb6f04a3",
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper()
                    },

                    new IdentityRole
                    {
                        Id = "6c0b1d22-4667-4533-8c39-da0619d01116",
                        Name = "User",
                        NormalizedName = "User".ToUpper()
                    }
                    );

                var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "a0a470e3-ca15-41fa-ac93-763a1a1201d2",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!"),
                        Bio = "Admin",
                        FirstName = "Admin",
                        LastName = "Admin",
                        IsPublic = true
                    },

                    new ApplicationUser
                    {
                        Id = "7e1f9bc6-cb3e-4ecb-b604-7003ae87b4c6",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!"),
                        Bio = "User",
                        FirstName = "User",
                        LastName = "User",
                        IsPublic = true
                    }
                    );

                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "c62037fa-90d9-4fa7-8a27-e1e4cb6f04a3",
                        UserId = "a0a470e3-ca15-41fa-ac93-763a1a1201d2"
                    },

                    new IdentityUserRole<string>
                    {
                        RoleId = "6c0b1d22-4667-4533-8c39-da0619d01116",
                        UserId = "7e1f9bc6-cb3e-4ecb-b604-7003ae87b4c6"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}