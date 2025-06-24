
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Data;
using AutodietaSemanal.Models;
using AutodietaSemanal.Services;

namespace AutodietaSemanal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AutodietaContext _context;
        private readonly DietaService _dietaService;
        
        public AccountController(AutodietaContext context, DietaService dietaService)
        {
            _context = context;
            _dietaService = dietaService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine("Intentando loguear al usuario: " + model.NombreUsuario);
            Console.WriteLine($"Model: Usuario={model.NombreUsuario}, Contraseña={model.Contraseña}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelo inválido en Login");
                return View(model);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario && u.Contraseña == model.Contraseña);

            if (usuario == null)
            {
                Console.WriteLine("Credenciales inválidas");
                ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos");
                return View(model);
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("EsAdministrador", usuario.EsAdministrador.ToString());

            Console.WriteLine($"Inicio de sesión exitoso: {usuario.NombreUsuario} (Admin: {usuario.EsAdministrador})");

            return usuario.EsAdministrador
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Usuario");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelo inválido en Register");
                return View(model);
            }

            // Verificar si el usuario ya existe
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == model.NombreUsuario);

            if (usuarioExistente)
            {
                Console.WriteLine("Usuario ya existe");
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso");
                return View(model);
            }

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = model.NombreUsuario,
                Contraseña = model.Contraseña,
                Email = model.Email,
                EsAdministrador = false,
                FechaVencimientoDieta = DateTime.Now.AddDays(7), // Una semana por defecto
                FechaCreacion = DateTime.Now
            };
            
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();
            Console.WriteLine("Usuario guardado correctamente");


            // Generar dieta inicial
            await _dietaService.GenerarDietaSemanalAsync(nuevoUsuario.Id);
            
            // Iniciar sesión automáticamente
            HttpContext.Session.SetInt32("UsuarioId", nuevoUsuario.Id);
            HttpContext.Session.SetString("NombreUsuario", nuevoUsuario.NombreUsuario);
            HttpContext.Session.SetString("EsAdministrador", "False");
            
            return RedirectToAction("Index", "Usuario");
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
