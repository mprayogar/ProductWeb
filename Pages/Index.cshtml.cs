using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWeb.models;

namespace ProductWeb.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public List<Product> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login"); 
            }

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:7237");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = "/api/product/filter";

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(SearchKeyword))
                queryParams.Add($"keyword={Uri.EscapeDataString(SearchKeyword)}");
            if (MinPrice.HasValue)
                queryParams.Add($"minPrice={MinPrice.Value}");
            if (MaxPrice.HasValue)
                queryParams.Add($"maxPrice={MaxPrice.Value}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);


            var httpResponse = await client.GetAsync(url);

            

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Login"); 
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                
                Products = new List<Product>();
                return Page();
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<List<Product>>>();
            Products = response?.Data ?? new List<Product>();



            return Page();
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7237") // Ganti jika pakai HTTP
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/product/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Produk berhasil dihapus.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Gagal menghapus produk: {error}";
            }

            return RedirectToPage(); 
        }



    }
}

