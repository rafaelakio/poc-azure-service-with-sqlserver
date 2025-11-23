using Microsoft.EntityFrameworkCore;
using LinkManager.Web.Data;
using LinkManager.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configuração do Entity Framework com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Registro dos repositórios (DAL)
builder.Services.AddScoped<IPageLinkRepository, PageLinkRepository>();

// Registro dos serviços
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();

// Configuração do HttpClient para o HealthChecker
builder.Services.AddHttpClient();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Aplica migrations automaticamente no startup (apenas para desenvolvimento)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        await dbContext.Database.MigrateAsync();
        app.Logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.Run();
