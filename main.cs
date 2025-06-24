using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Data;
using AutodietaSemanal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}"); // Para debug

builder.Services.AddDbContext<AutodietaContext>(options =>
    options.UseSqlServer(connectionString));

// Session - Configuración más robusta
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AutodietaSemanal.Session";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Custom services
builder.Services.AddScoped<DietaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// ORDEN CRÍTICO DE MIDDLEWARES
//app.UseHttpsRedirection(); //  lo tengo comentado porque si no no me carga la pagina
app.UseStaticFiles();

app.UseRouting();

// La sesión DEBE ir después de UseRouting y antes de UseAuthorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Crear la base de datos automáticamente si no existe con manejo de errores
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AutodietaContext>();

        // Probar la conexión primero
        Console.WriteLine("Probando conexión a la base de datos...");
        await context.Database.CanConnectAsync();
        Console.WriteLine(" Conexión exitosa!");

        // Crear la BD si no existe
        bool created = await context.Database.EnsureCreatedAsync();
        if (created)
        {
            Console.WriteLine(" Base de datos creada exitosamente!");
        }
        else
        {
            Console.WriteLine(" La base de datos ya existe.");
        }

        // Verificar que el usuario admin existe
        var adminExists = await context.Usuarios.AnyAsync(u => u.NombreUsuario == "admin");
        if (adminExists)
        {
            Console.WriteLine("Usuario administrador encontrado.");
        }
        else
        {
            Console.WriteLine("Usuario administrador no encontrado. Verificar seed data.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error de conexión a la base de datos: {ex.Message}");
    Console.WriteLine($"Detalles: {ex.InnerException?.Message}");
    // No detener la aplicación, pero registrar el error
}

Console.WriteLine("Aplicación iniciando en http://localhost:5001");
app.Run("http://localhost:5001");