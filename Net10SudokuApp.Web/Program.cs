using Net10SudokuApp.Web;
using Net10SudokuApp.Web.Components;
using Net10SudokuApp.Web.Application.Interfaces;
using Net10SudokuApp.Web.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations (only in development)
if (builder.Environment.IsDevelopment())
{
    builder.AddServiceDefaults();
}

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Razor Pages for .cshtml hosting
builder.Services.AddRazorPages();

builder.Services.AddOutputCache();

// Configure API client for all environments, using environment-based URL
var apiServiceUrl = builder.Configuration["ApiService:BaseUrl"] ?? "https://apiservice";
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri(apiServiceUrl);
});

// Register Sudoku services
builder.Services.AddSingleton<ISudokuGenerator, SudokuService>();
builder.Services.AddSingleton<ISudokuSolver, SudokuService>();
builder.Services.AddSingleton<ISudokuValidator, SudokuService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();

// Map endpoints in correct order
// 1. Static assets (CSS, JS, etc.)
app.MapStaticAssets();

// 2. Default endpoints (health checks, etc.) - only in development
if (app.Environment.IsDevelopment())
{
    app.MapDefaultEndpoints();
}

// 3. Razor Pages (for .cshtml files) - must come before RazorComponents to avoid conflicts
app.MapRazorPages();

// 4. Razor Components (Blazor) - this automatically includes Blazor hub mapping
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
