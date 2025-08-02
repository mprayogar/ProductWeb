using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ProductWeb.models;
using Microsoft.AspNetCore.Identity;

namespace ProductWeb.Pages;

public class Login : PageModel
{
    [BindProperty] public bool IsRegistering { get; set; }
    public string? SuccessMessage { get; set; }

    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet() => ErrorMessage = null;

    public async Task<IActionResult> OnPostAsync()
    {
        if (Request.Form.ContainsKey("ToggleRegister"))
        {
            IsRegistering = true; 
            return Page();
        }

        if (Request.Form.ContainsKey("ToggleLogin"))
        {
            IsRegistering = false;  
            return Page();
        }

        
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7237")  // Sesuaikan dengan URL API
        };

        if (IsRegistering)
        {
            var registerRequest = new models.RegisterRequest
            {
                Username = Username,
                Password = Password
            };

            var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Registrasi berhasil! Silakan klik 'Sudah punya akun? Login' untuk masuk.";
                Username = string.Empty;
                Password = string.Empty;
                ModelState.Clear(); 
                IsRegistering = true;

                return Page();
            }

            var result = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Registrasi gagal: {result}";
            IsRegistering = true; // tetap di halaman register walaupun gagal
            return Page();
        }


        else
        {
            // Login
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




}


