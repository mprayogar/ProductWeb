using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWeb.models;
using ProductWeb.Services;

namespace ProductWeb.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ProductApiService _api;

        public ProductsModel(ProductApiService api)
        {
            _api = api;
        }

        public List<Product> Products { get; set; } = new();
        [BindProperty]
        public Product NewProduct { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await _api.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _api.AddAsync(NewProduct);
            return RedirectToPage();
        }
    }
}
