using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Need4Chat.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IConfigurationRoot builder = new ConfigurationBuilder().AddCommandLine(args).Build();
            IWebHost host = WebHost.CreateDefaultBuilder(args).UseConfiguration(builder).UseStartup<Startup>().Build();
            return host;
        }
    }
}
