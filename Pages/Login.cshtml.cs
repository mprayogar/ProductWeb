using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ProductWeb.models;

namespace ProductWeb.Pages;

public class Login : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet() => ErrorMessage = null;

    public async Task<IActionResult> OnPostAsync()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7237")
        };

        var loginRequest = new models.LoginRequest
        {
            Username = Username,        
            Password = Password
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                HttpContext.Session.SetString("token", loginResponse.AccessToken);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Gagal mendapatkan token dari server.";
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Login gagal: {errorContent}";
        }


        return Page();
    }
}


