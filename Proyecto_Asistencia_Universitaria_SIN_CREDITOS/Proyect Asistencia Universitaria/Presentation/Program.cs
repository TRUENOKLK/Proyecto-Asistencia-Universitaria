using Aplication.Interface;
using Aplication.Service;
using Infrastructure.DataBase;
using Infrastructure.Repository;
using Infrastructure.Reportes;
using Infrastructure.Seguridad;
using Microsoft.AspNetCore.Authentication.Cookies;


QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);


var conexion = builder.Configuration.GetConnectionString("defaultconection");
if (string.IsNullOrWhiteSpace(conexion))
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'defaultconection'. Revisa appsettings.json " +
        "o appsettings.Development.json (ver README.md para más detalles).");

builder.Services.AddSingleton(new DBConnectionFactory(conexion));


builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<EstudianteServices>();
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<CarreraServices>();
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<FacultadServices>();
builder.Services.AddScoped<IProfesorRepository, ProfesorRepository>();
builder.Services.AddScoped<ProfesorServices>();


builder.Services.AddScoped<IPeriodoRepository, PeriodoRepository>();
builder.Services.AddScoped<PeriodoServices>();
builder.Services.AddScoped<IAsignaturaRepository, AsignaturaRepository>();
builder.Services.AddScoped<AsignaturaServices>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepository>();
builder.Services.AddScoped<GrupoServices>();
builder.Services.AddScoped<IReportePdfService, ReportePdfService>();


builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();


builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICorreoInstitucionalGenerator, CorreoInstitucionalGenerator>();


builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<AuditoriaServices>();
builder.Services.AddScoped<ISesionRepository, SesionRepository>();
builder.Services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
builder.Services.AddScoped<OperacionesServices>(); 


builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "AsistenciaUni.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();


builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
