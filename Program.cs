using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ServerStatus
{
	/// <summary>
	/// main program entry pt
	/// </summary>
	public class Program
    {
		/// <summary>
		/// mainline
		/// </summary>
		/// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

		/// <summary>
		/// build the web host object
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .Build();
    }
}
