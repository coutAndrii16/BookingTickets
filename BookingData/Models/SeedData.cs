using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookingData.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            EventDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<EventDbContext>();

            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();

            if (!context.Events.Any())
            {
                var concert = new Event
                {
                    Title = "Концерт",
                    Description = "Живий концерт популярних виконавців",
                    Date = DateTime.Now.AddDays(5),
                    Location = "Kyiv",
                    Price = 500
                };
                var conference = new Event
                {
                    Title = "IT Конференція",
                    Description = "Щорічна конференція розробників",
                    Date = DateTime.Now.AddDays(10),
                    Location = "Lviv",
                    Price = 300
                };
                var festival = new Event
                {
                    Title = "Кінофестиваль",
                    Description = "Міжнародний кінофестиваль",
                    Date = DateTime.Now.AddDays(15),
                    Location = "Odesa",
                    Price = 200
                };

                context.Events.AddRange(concert, conference, festival);
                context.SaveChanges();

                context.Tickets.AddRange(
                    new Ticket
                    {
                        BuyerName = "Іван Петренко",
                        BuyerEmail = "ivan@example.com",
                        SeatCount = 2,
                        OrderDate = DateTime.Now,
                        EventId = concert.EventId!.Value
                    },
                    new Ticket
                    {
                        BuyerName = "Марія Коваль",
                        BuyerEmail = "maria@example.com",
                        SeatCount = 1,
                        OrderDate = DateTime.Now,
                        EventId = conference.EventId!.Value
                    }
                );
                context.SaveChanges();
            }
        }

        public static async Task EnsureRolesAndAdminAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create default admin
            const string adminEmail = "admin@booking.com";
            const string adminPassword = "Admin123!";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
