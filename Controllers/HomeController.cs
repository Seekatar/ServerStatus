using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ServerStatus.Controllers
{
	/// <summary>
	/// controller for UI 
	/// </summary>
	public class HomeController : Controller
    {
		/// <summary>
		/// Get the Continuum and Zabbix HTML page
		/// </summary>
		/// <param name="count">count of items to return, defaults to 20, max 50</param>
		/// <returns>Continuum and Zabbix HTML page</returns>
        [Route("{count?}")]
		[HttpGet]
        public IActionResult Index(int count=20)
        {
            return View();
        }

		/// <summary>
		/// Get the Error
		/// </summary>
		/// <returns></returns>
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
