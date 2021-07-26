using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Defect_Tracker.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("Defect_Tracker.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Defect_Tracker.ServerAPI"));

            // This allows anonymous requests.
            builder.Services.AddHttpClient("ServerAPI.NoAuthenticationClient",
            client => client.BaseAddress = new
           Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddApiAuthorization();

            // Syncfusion support
            builder.Services.AddSyncfusionBlazor();


            await builder.Build().RunAsync();
        }
    }
}
