using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Need4Chat.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services
              .AddBlazorise(options =>
              {
                  options.ChangeTextOnKeyPress = true;
              })
              .AddBootstrapProviders()
              .AddFontAwesomeIcons();

            builder.Services.AddBaseAddressHttpClient();

            builder.RootComponents.Add<App>("app");

            WebAssemblyHost host = builder.Build();

            host.Services
              .UseBootstrapProviders()
              .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
