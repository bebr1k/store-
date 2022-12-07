using Store;
using Store.Contractors;
using Store.Messages;
using Store.Web.Contractors;
using Store.YandexKassa;
using Store.Web.App;
using Store.Data.EF;
using System.Configuration;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEfRepositories(builder.Configuration.GetConnectionString("Store"));
builder.Services.AddScoped<Dictionary <Type, StoreDbContext>>();
builder.Services.AddSingleton<DbContextFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IPaymentService, YandexKassaPaymentService>();
builder.Services.AddSingleton<IWebContractorService, YandexKassaPaymentService>();
builder.Services.AddSession(options=>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IDeliveryService,PostamateDeliveryService>();
builder.Services.AddSingleton<IPaymentService, CashPaymentService>();
builder.Services.AddSingleton<INotificationService, DebugNotificationService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<BookService>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "yandex.kassa", 
        areaName: "YandexKassa", 
        pattern: "YandexKassa/{controller=Home}/{action=Index}/{id?}");


});
app.Run();
