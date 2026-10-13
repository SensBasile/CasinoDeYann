using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });

// Clés qui chiffrent le cookie de connexion et les jetons anti-CSRF. Par défaut
// elles vivent dans le conteneur et disparaissent à chaque redéploiement (tout
// le monde est déconnecté) : en Docker, on les range dans un volume.
var keysPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrEmpty(keysPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName("CasinoDeYann");
}

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

// Applique les migrations en attente au démarrage (sans effet si la base est à jour).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<CasinoDbContext>().Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// En prod, le HTTPS est géré par le nginx frontal : la redirection ne sert qu'en local.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers();

app.Run();
