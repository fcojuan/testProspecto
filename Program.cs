using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Prospecto.Data;
using Prospecto.Repositorio;
using Microsoft.EntityFrameworkCore;
using Prospecto.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<ProspectoContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//Para login
builder.Services.AddIdentity<ProspectoUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddEntityFrameworkStores<ProspectoContext>()
    .AddDefaultTokenProviders(); // es necesario si modificas Usuario Identity();

var sqlConnectionConfiguration = new ConnectionConfiguration(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(sqlConnectionConfiguration);
builder.Services.AddScoped(typeof(IRepository<>), typeof(CRepository<>));

//Reclamo de usuarios
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ProspectoUser>, MyUserClaimsPrincipalFactory>();

builder.Services.AddRazorPages();
//-------------USUARIOS------------------------------
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configuraciones de contrase�a
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 6;
    options.SignIn.RequireConfirmedAccount = false;//confirmacion del correo electronico

    // Configuraciones de bloqueo
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(25);//Cerras Sesion en minutos
    options.Lockout.MaxFailedAccessAttempts = 1;
    options.Lockout.AllowedForNewUsers = true;

    // Ajustes de usuario
    options.User.RequireUniqueEmail = true;
});

//la comprobaci�n de si el usuario est� conectado ocurre ~cada 10 segundos
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = 4096;
});
builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromSeconds(25));
//Establecer la p�gina de inicio de sesi�n de la cuenta 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "ConFunT";
    //options.Cookie.Expiration = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
    //"/Identity/Pages/Account/Login"
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    // ReturnUrlParameter requirido 
    //usando Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});
//Variable de session
builder.Services.AddDistributedMemoryCache(); //Agrega una implementaci�n predeterminada en memoria de IDistributedCache
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(25);// caducidad de la sesi�n en  minuto
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
