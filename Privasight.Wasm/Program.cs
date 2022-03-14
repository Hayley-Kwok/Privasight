using Privasight.Wasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Privasight.Wasm.Services;
using Radzen;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Privasight.Model.Facebook.Data;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<ConfigService>();

// Sets up EF Core with Sqlite
builder.Services.AddDbContextFactory<FbContext>(options =>
    options
        .UseSqlite($"Filename={DataAccessHelper.SqliteDbFilename}")
        .EnableSensitiveDataLogging());

await builder.Build().RunAsync();