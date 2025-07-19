using Melodix.Data;
using Melodix.Models.Models;
using Melodix.MVC.Services.Patterns.Adapter.SendGrid;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Melodix.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Configura Kestrel para usar el certificado generado por mkcert
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5001, listenOptions =>
                {
                    listenOptions.UseHttps(
                        Path.Combine(AppContext.BaseDirectory, "localhost.pfx")
                    );
                });
            });

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages();

            // Usa solo AddTransient o AddSingleton, pero no ambos
            builder.Services.AddTransient<IMailProvider, SendGridAdapter>();
            builder.Services.AddTransient<IEmailSender, SendGridAdapter>();


            builder.Services.Configure<SpotifyOptions>(builder.Configuration.GetSection("Spotify"));
            builder.Services.AddScoped<SpotifyService>();

            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 5001;
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 15;
                options.Password.RequiredUniqueChars = 1;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}