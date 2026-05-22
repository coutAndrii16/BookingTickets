using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BookingBlazor.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;
        private readonly HttpClient http;

        public JwtAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            this.localStorage = localStorage;
            this.http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // Check expiration
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt;
            try { jwt = handler.ReadJwtToken(token); }
            catch { return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); }

            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyAuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}