using ApiClient.Client;
using BlazorWASMOpenAPI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

const string clientId = "Salman";
const string clientSecret = @"12345";

var authenticationString = $"{clientId}:{clientSecret}";
var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));


builder.Services.AddScoped(sp =>
    new Client(new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) ,
        DefaultRequestHeaders =
        {
            Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString)
        }
    }));



await builder.Build().RunAsync();
