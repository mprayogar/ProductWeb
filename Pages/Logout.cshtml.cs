using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ProductWeb.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPost()
        {
            _logger.LogInformation("Logout dipanggil");
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}
