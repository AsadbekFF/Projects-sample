using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectWithAuthenticationSample.Infrastructure;
using ProjectWithAuthenticationSample.Infrastructure.Authentication;
using Sample.BLL.Services;
using Sample.Common.Logging;
using Sample.Common.Models;
using Sample.DAL.Infrastructure;
using System.Security.Principal;
using ILogger = Sample.Common.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

var settings = new Settings();
builder.Configuration.Bind(settings);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
        cookieContext.CookieOptions.SameSite = SameSiteMode.None;
    options.OnDeleteCookie = cookieContext =>
        cookieContext.CookieOptions.SameSite = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAntiforgery(antiforgeryOptions =>
{
    antiforgeryOptions.SuppressXFrameOptionsHeader = true;
});

builder.Services.AddDbContext<MasterContext>(options => options.UseSqlServer(settings.ConnectionString.DefaultConnection));

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/user/log-in";
                    options.LogoutPath = "/user/log-out";
                    options.AccessDeniedPath = "/error/403";
                    options.SlidingExpiration = false; // expiration set explicitly
                    options.Events.OnValidatePrincipal = PrincipalCookieValidator.ValidatePrincipal;
                });

builder.Services.AddScoped<IPrincipal>(implementationFactory: sp => sp.GetService<IHttpContextAccessor>()?.HttpContext?.User);
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<ILogger, NLogLogger>();
builder.Services.AddScoped<IMapper, AutoMapperMapper>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MasterContext>();
var logger = app.Services.GetRequiredService<ILogger>();

// Migrate database
context.Database.Migrate();
// Must-have data
context.Initialize();

app.Run();
