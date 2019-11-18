using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Guardian.Web.UI
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
