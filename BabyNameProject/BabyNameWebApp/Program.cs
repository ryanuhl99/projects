using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BabyNameWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

var dbServer = Environment.GetEnvironmentVariable("SERVER");
var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbName = "BabyNames";

var connectionString = $"Server={dbServer};Database={dbName};User Id={dbUsername};Password={dbPassword};TrustServerCertificate=True;";

builder.Configuration["ConfigurationStrings:DefaultConnection"] = connectionString;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<BabyNamesContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
