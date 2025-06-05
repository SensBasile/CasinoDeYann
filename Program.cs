using CasinoDeYann.Src;
using CasinoDeYann.Src.DataAccess;
using CasinoDeYann.Src.DataAccess.EFModels;
using CasinoDeYann.Src.DataAccess.Interfaces;
using CasinoDeYann.Src.Services;
using CasinoDeYann.Src.Services.GoldMineService;
using CasinoDeYann.Src.Services.Roulette;
using CasinoDeYann.Src.Services.SlotMachine;
using CasinoDeYann.Src.Services.Stats;
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
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SlotMachineService>();
builder.Services.AddScoped<RouletteService>();
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