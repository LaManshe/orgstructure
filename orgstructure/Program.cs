using orgstructure.Data;
using orgstructure.Repository;
using orgstructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEFUsersRepository, EFUsersRepository>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Context>(
        dbContextOptions => dbContextOptions.UseNpgsql(builder.Configuration.GetConnectionString("Pgl_db"))
    );

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
