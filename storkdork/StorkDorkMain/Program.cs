using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

        var conStrBuilder = new SqlConnectionStringBuilder(
                builder.Configuration.GetConnectionString("StorkDorkDB"));
        var connectionString = conStrBuilder.ConnectionString;

        builder.Services.AddDbContext<StorkDorkContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

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

        app.Run();
    }
}