using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ProductWeb.models;

namespace ProductWeb.Pages
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public Product Product { get; set; }

         public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
               
                return RedirectToPage("/Login");
            }

            return Page(); 
        }


        public async Task<IActionResult> OnPostAsync()
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

            var response = await client.PostAsJsonAsync("/api/product", Product);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Produk berhasil ditambahkan.";
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Gagal menambahkan produk.");
            return Page();
        }
    }
}
