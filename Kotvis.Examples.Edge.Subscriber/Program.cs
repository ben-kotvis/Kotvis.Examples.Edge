namespace subscriber
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;

    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AllowSynchronousIO = false;
                        options.ListenAnyIP(8081);
                    });
                    webBuilder.UseStartup<Startup>();
                });

    }
}
