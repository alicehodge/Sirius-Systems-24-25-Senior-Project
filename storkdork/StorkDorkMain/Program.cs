

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Data;
using Microsoft.AspNetCore.Identity;
using StorkDork.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using StorkDorkMain.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Needed for identity ui to route properly
        builder.Services.AddRazorPages();

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

        //StorkDork database setup
        var conStrBuilder = new SqlConnectionStringBuilder(
            builder.Configuration.GetConnectionString("StorkDorkDB"));
        var connectionString = conStrBuilder.ConnectionString;

        builder.Services.AddDbContext<StorkDorkContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

        //Identity database setup
        var conStrBuilderTwo = new SqlConnectionStringBuilder(
            builder.Configuration.GetConnectionString("IdentityDB"));
        var connectionStringIdentity = conStrBuilderTwo.ConnectionString;

        builder.Services.AddDbContext<StorkDorkIdentityDbContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionStringIdentity)
        );


        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<StorkDorkIdentityDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddScoped<DbContext, StorkDorkContext>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IBirdRepository, BirdRepository>();
        builder.Services.AddScoped<ISightingService, SightingService>();

        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "Leaflet",
            pattern: "Leaflet/{action=Index}/{id?}",
            defaults: new { controller = "Leaflet" });

        app.MapControllerRoute(
            name: "search",
            pattern: "Search/{action=Index}/{id?}",
            defaults: new { controller = "Search" });

        app.MapControllerRoute(
            name: "BirdLog",
            pattern: "BirdLog/{action=Index}/{id?}",
            defaults: new { controller = "BirdLog" });

        app.MapControllerRoute(
            name: "Bird",
            pattern: "Bird/{action=Index}/{id?}",
            defaults: new { controller = "Bird" });

        // Needed for identity ui routing to work
        app.MapRazorPages();

        app.Run();
    }
}

// Dummy email to satisfy identity requiring email sending. WILL DELETE LATER 
public class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask; // Does nothing, but satisfies the requirement
    }
}