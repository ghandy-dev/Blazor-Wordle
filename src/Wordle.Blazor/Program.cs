using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using System.Net.Http.Json;

using Wordle.Blazor;
using Wordle.Blazor.Game;
using Wordle.Blazor.Services;
using Wordle.Blazor.Utilities;
using Wordle.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

var words = await httpClient.GetFromJsonAsync<string[]>("data/words.json");
var dictionary = await httpClient.GetFromJsonAsync<string[]>("data/dictionary.json");

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => httpClient);
builder.Services.AddScoped<IKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IWordProvider>(sp => new WordProvider(dictionary!, words!));
builder.Services.AddScoped<IKeyboardService>(sp => KeyboardService.Instance);
builder.Services.AddScoped<GameService>();
builder.Services.AddTransient<IGameStorageService, GameStorageService>();
builder.Services.AddBlazoredLocalStorage();

if (builder.HostEnvironment.IsDevelopment())
{
    Console.WriteLine("development");
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

var host = builder.Build();

await host.RunAsync();