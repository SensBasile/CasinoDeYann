using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using CasinoDeYann;
using CasinoDeYann.DataAccess;
using CasinoDeYann.DataAccess.EFModels;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services;
using CasinoDeYann.Services.GoldMineService;
using CasinoDeYann.Services.HorseRace;
using CasinoDeYann.Services.Roulette;
using CasinoDeYann.Services.SlotMachine;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<CasinoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(typeof(AutomapperProfiles));

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IRouletteService, RouletteService>();
builder.Services.AddScoped<ISlotMachineService, SlotMachineService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<GoldMineService>();
builder.Services.AddScoped<HorseRaceService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
