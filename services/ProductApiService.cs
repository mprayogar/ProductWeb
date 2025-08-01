using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using ProductWeb.models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace ProductWeb.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _http;

        public ProductApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                var httpResponse = await _http.GetAsync("/api/product");

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Bisa return null atau lempar custom exception
                    return null;
                }

                httpResponse.EnsureSuccessStatusCode();

                var result = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<List<Product>>>();
                return result?.Data ?? new();
            }
            catch
            {
                // Log atau tangani error lain jika perlu
                return new();
            }
        }



        public async Task AddAsync(Product product)
        {
            await _http.PostAsJsonAsync("/api/product", product);
        }

        public void SetAuthorizationToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }


    }
}
