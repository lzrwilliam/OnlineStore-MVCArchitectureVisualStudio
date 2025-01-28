using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ProiectPAW.ContextModels;
using ProiectPAWMvc.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // Pentru a accesa HttpContext in CustomRoleProvider

builder.Services.AddSingleton<IAuthorizationHandler, CustomRoleProvider>(); // adauga, clasa customroleprovider, o inregistram drept i authorization


builder.Services.AddDbContext<ProdusContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MagazinOnline1")));
//dependency injection pt a folozi baza de date si pentru a o updata cand facem add migration si update database

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Authentication/Login";
                    options.AccessDeniedPath = "/Authentication/Forbidden/"; 
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
});

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions //pentru a folosi folderul src pt imagini statice
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "src")),
    RequestPath = "/src"
});



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
app.UseAuthentication(); // de ce am adaugat o ???

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
