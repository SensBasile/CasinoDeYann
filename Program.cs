using CasinoDeYann.Api.DataAccess;
using CasinoDeYann.Api.DataAccess.EFModels;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services;
using CasinoDeYann.Api.Services.GoldMineService;
using CasinoDeYann.Api.Services.SlotMachine;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<CasinoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });

builder.Services.AddAutoMapper(typeof(AutomapperProfiles));

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<SlotMachineService>();
builder.Services.AddScoped<GoldMineService>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers();



app.Run();