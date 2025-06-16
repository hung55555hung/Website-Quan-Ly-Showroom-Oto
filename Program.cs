using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebPBL3.Models;
using WebPBL3.Services;

var builder = WebApplication.CreateBuilder(args);
// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.HttpOnly = true;
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/Denied";
        option.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("Staff", policy => policy.RequireRole("Staff"));
	options.AddPolicy("Admin,Staff", policy =>
	{
		policy.RequireRole("Admin");
		policy.RequireRole("Staff");
	});
});

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(3);
});
// Register the INewsService and NewsService

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
