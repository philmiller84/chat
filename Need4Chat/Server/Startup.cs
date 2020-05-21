using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;

namespace Need4Chat.Server
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            //string dbConn = Configuration.GetSection("ConnectionString").GetSection("Burbank").Value;
            //services.AddDbContext<database1Context>(opt => opt.UseSqlServer(dbConn), ServiceLifetime.Transient);

            services.AddBlazorise(options => { options.ChangeTextOnKeyPress = true; })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddMvc().AddNewtonsoftJson();
            services.AddSignalR();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();  // preview2 change

            app.UseRouting();


            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                // SignalR endpoint routing setup
                endpoints.MapHub<Hubs.TradeHub>(Shared.TradeClient.HUBURL);
                endpoints.MapHub<Hubs.ChatHub>(Shared.ChatClient.HUBURL);
                endpoints.MapHub<Hubs.LoginHub>(Shared.LoginClient.HUBURL);

                endpoints.MapFallbackToFile("index.html");  // preview2 change
            });
        }

    }
}
