using System.Globalization;
using DimDim.Web.Data;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.EntityFrameworkCore;

// Ponto como separador decimal no saldo, igual no Windows e no Linux do App Service
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// A string de conexão NÃO fica no código: no Azure ela vem da Connection String "DimDimDb"
// configurada no Web App (variável SQLAZURECONNSTR_DimDimDb), criada pelo script do CLI.
var connectionString = builder.Configuration.GetConnectionString("DimDimDb")
    ?? throw new InvalidOperationException("Connection string 'DimDimDb' não configurada.");

builder.Services.AddDbContext<DimDimContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

// Application Insights: lê APPLICATIONINSIGHTS_CONNECTION_STRING das App Settings.
// Requisições, exceções e cada comando SQL enviado ao Azure SQL viram telemetria.
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
    (module, _) => module.EnableSqlCommandTextInstrumentation = true);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
