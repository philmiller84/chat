using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Need4Chat.Shared.DataLookups;
using Need4Chat.Shared.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Need4Chat.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSingleton<IQueryData, TradeDataLookup>();
            builder.Services
              .AddBlazorise(options =>
              {
                  options.ChangeTextOnKeyPress = true;
              })
              .AddBootstrapProviders()
              .AddFontAwesomeIcons();


            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.RootComponents.Add<App>("app");

            WebAssemblyHost host = builder.Build();

            host.Services.UseBootstrapProviders().UseFontAwesomeIcons();
            IQueryData dataQueryService = host.Services.GetRequiredService<IQueryData>();

            await host.RunAsync();
        }
    }
}
