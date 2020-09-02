using BysykkelData;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bysykkel.Controllers
{
    public class HomeController : Controller
    {
        private readonly BysykkelService _bysykkelService;

        public HomeController(BysykkelService bysykkelService)
        {
            _bysykkelService = bysykkelService;
        }

        /// <summary>
        ///     List bike stations with number of bikes available and number of locks.
        /// </summary>
        /// <param name="id">If specified, only show stations that have id as part of their name</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string id)
        {
            var data = await _bysykkelService.GetData(id);
            return View(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorMessage = HttpContext.Features.Get<IExceptionHandlerPathFeature>().Error.Message;
            return View("Error", errorMessage);
        }
    }
}
