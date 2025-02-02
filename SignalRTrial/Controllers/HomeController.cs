using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignalRTrial.Configurations;
using SignalRTry.ViewModels;

namespace SignalRTry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExternalSiteSettings _externalSiteSettings;

        public HomeController(IOptions<ExternalSiteSettings> externalSiteSettings)
        {
            _externalSiteSettings = externalSiteSettings.Value;
        }

        public IActionResult Index()
        {
            return View(new HomeViewModel() { ReactSite = _externalSiteSettings.ReactSite });
        }
    }
}
