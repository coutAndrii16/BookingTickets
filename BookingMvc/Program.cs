using BookingData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Main DB
builder.Services.AddDbContext<EventDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:EventDbConnection"]));

// Identity DB
builder.Services.AddDbContext<AppIdentityDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]));

// Identity with Roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    // Password requirements
    opts.Password.RequiredLength = 8;
    opts.Password.RequireDigit = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireNonAlphanumeric = false;
    // Unique email
    opts.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Login";
    opts.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IEventRepository, EFEventRepository>();
builder.Services.AddScoped<ITicketRepository, EFTicketRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

SeedData.EnsurePopulated(app);
await SeedData.EnsureRolesAndAdminAsync(app);

app.Run();
