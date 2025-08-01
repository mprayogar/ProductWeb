using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ProductWeb.models;

namespace ProductWeb.Pages
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("token");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7237")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await client.GetAsync($"/api/product/{id}");

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Login"); // atau ke route login yang kamu pakai
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                // Opsional: handle error lain (misalnya 404, 500, dll)
                ModelState.AddModelError(string.Empty, "Gagal mengambil data produk.");
                return Page();
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<Product>>();
            Product = response?.Data ?? new Product();


            if (response == null || response.Data == null)
                return NotFound();

            Product = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("token");

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7237")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"/api/product/{Product.Id}", Product);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Produk berhasil diperbarui.";
                return RedirectToPage("/Index");
            }

            // Tambahkan ini untuk menampilkan pesan error dari API
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Gagal update produk: {errorContent}");

            return Page();
        }

    }
}
