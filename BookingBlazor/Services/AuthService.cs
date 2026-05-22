using Blazored.LocalStorage;
using BookingBlazor.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace BookingBlazor.Services
{
    public class AuthService
    {
        private readonly HttpClient http;
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authStateProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            this.http = http;
            this.localStorage = localStorage;
            this.authStateProvider = authStateProvider;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            var response = await http.PostAsJsonAsync("api/auth/register", new
            {
                dto.Email,
                dto.Password,
                dto.Role
            });

            if (response.IsSuccessStatusCode)
                return (true, "Реєстрацію успішно завершено!");

            var error = await response.Content.ReadAsStringAsync();
            return (false, $"Помилка реєстрації: {response.StatusCode}");
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginDto dto)
        {
            var response = await http.PostAsJsonAsync("api/auth/login", dto);

            if (!response.IsSuccessStatusCode)
                return (false, "Невірний email або пароль");

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result == null) return (false, "Помилка відповіді сервера");

            await localStorage.SetItemAsync("authToken", result.Token);
            ((JwtAuthStateProvider)authStateProvider).NotifyAuthStateChanged();

            return (true, "Вхід успішний!");
        }

        public async Task LogoutAsync()
        {
            await localStorage.RemoveItemAsync("authToken");
            ((JwtAuthStateProvider)authStateProvider).NotifyAuthStateChanged();
        }
    }
}
