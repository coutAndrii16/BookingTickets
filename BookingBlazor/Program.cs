using Blazored.LocalStorage;
using BookingBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<BookingBlazor.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register handler
builder.Services.AddScoped<ApiHttpHandler>();

// HttpClient with base address
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<ApiHttpHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7200/")
    };
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();