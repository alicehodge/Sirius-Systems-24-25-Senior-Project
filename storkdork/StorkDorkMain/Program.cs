

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Data;
using Microsoft.AspNetCore.Identity;
using StorkDork.Areas.Identity.Data;

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

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

        // StorkDork database setup
        var conStrBuilder = new SqlConnectionStringBuilder(
            builder.Configuration.GetConnectionString("StorkDorkDB"));
        var connectionString = conStrBuilder.ConnectionString;

        builder.Services.AddDbContext<StorkDorkContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

        // Identity database setup
        var conStrBuilderTwo = new SqlConnectionStringBuilder(
            builder.Configuration.GetConnectionString("IdentityDB"));
        var connectionStringIdentity = conStrBuilderTwo.ConnectionString;

        builder.Services.AddDbContext<StorkDorkIdentityDbContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionStringIdentity)
        );

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<StorkDorkIdentityDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddScoped<DbContext, StorkDorkContext>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IBirdRepository, BirdRepository>();

        builder.Services.AddSwaggerGen();

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

        app.Run();
    }
}
