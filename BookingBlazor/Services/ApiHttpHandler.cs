using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace BookingBlazor.Services
{
    public class ApiHttpHandler : DelegatingHandler
    {
        private readonly ILocalStorageService localStorage;

        public ApiHttpHandler(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}