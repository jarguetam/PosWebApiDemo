using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pos.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseSentry(o =>
                    //{
                    //    o.Dsn = "https://6d2c178bfeb880509c265e2579e34e3e@o4506513319198720.ingest.sentry.io/4506513321230336";
                    //    // When configuring for the first time, to see what the SDK is doing:
                    //    o.Debug = true;
                    //    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                    //    // We recommend adjusting this value in production.
                    //    o.TracesSampleRate = 1.0;
                    //});
                });
    }
}
