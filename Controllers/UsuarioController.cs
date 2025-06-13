
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Data;
using AutodietaSemanal.Models;

namespace AutodietaSemanal.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AutodietaContext _context;
        
        public UsuarioController(AutodietaContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
            if (usuario == null || usuario.EsAdministrador)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Obtener dieta activa
            var dietaActiva = await _context.DietasSemanales
                .Include(d => d.DietasDiarias)
                    .ThenInclude(dd => dd.RecetaDesayuno)
                .Include(d => d.DietasDiarias)
                    .ThenInclude(dd => dd.RecetaComida)
                .Include(d => d.DietasDiarias)
                    .ThenInclude(dd => dd.RecetaCena)
                .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId.Value && d.EsActiva);
            
            ViewBag.Usuario = usuario;
            ViewBag.FechaVencimiento = usuario.FechaVencimientoDieta;
            
            return View(dietaActiva);
        }
        
        public async Task<IActionResult> Receta(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var receta = await _context.Recetas.FindAsync(id);
            if (receta == null)
            {
                return NotFound();
            }
            
            return View(receta);
        }
    }
}
