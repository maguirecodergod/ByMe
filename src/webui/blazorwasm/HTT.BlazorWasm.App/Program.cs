using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HTT.BlazorWasm.App;
using HTT.BlazorWasm.App.Services.Localization;
using HTT.BlazorWasm.App.Services.Layout;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using HTT.BlazorWasm.App.Components;
using HTT.BlazorWasm.App.Services.Icons;
using HTT.BlazorWasm.App.Services.Icons.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Core Services
builder.Services.AddLogging();
builder.Services.AddScoped<IToaster, NullToaster>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<LayoutService>();

// Icon Services
builder.Services.AddScoped<IIconProvider, JsonAssetIconProvider>();
builder.Services.AddScoped<IIconService, IconService>();

// Master Localization (JSON based)
builder.Services.AddScoped<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

// Point the HttpClient to the gateway
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

var host = builder.Build();

// Initialize Localization
var js = host.Services.GetRequiredService<IJSRuntime>();
var locService = host.Services.GetRequiredService<LocalizationService>();
var result = await js.InvokeAsync<string>("localStorage.getItem", "blazor-culture");
var cultureName = result ?? "en-US";
locService.Initialize(cultureName);

await host.RunAsync();
