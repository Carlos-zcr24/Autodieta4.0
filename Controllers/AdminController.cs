
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Data;
using AutodietaSemanal.Models;
using AutodietaSemanal.Services;


namespace AutodietaSemanal.Controllers
{
    public class AdminController : Controller
    {
        private readonly AutodietaContext _context;
        private readonly DietaService _dietaService;
        
        public AdminController(AutodietaContext context, DietaService dietaService)
        {
            _context = context;
            _dietaService = dietaService;
        }
        
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var esAdmin = HttpContext.Session.GetString("EsAdministrador") == "True";
            
            if (usuarioId == null || !esAdmin)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Obtener solo usuarios no administradores
            var usuarios = await _context.Usuarios
                .Where(u => !u.EsAdministrador)
                .OrderBy(u => u.NombreUsuario)
                .ToListAsync();
            
            return View(usuarios);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var esAdmin = HttpContext.Session.GetString("EsAdministrador") == "True";
            
            if (usuarioId == null || !esAdmin)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var modelo = new Usuario
            {
                FechaVencimientoDieta = DateTime.Now.AddDays(7) // Fecha por defecto: próxima semana
            };
            
            return View(modelo);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            // Validar fecha de vencimiento
            if (usuario.FechaVencimientoDieta.Date <= DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaVencimientoDieta", "La fecha de vencimiento debe ser superior a la fecha actual");
            }
            
            // Verificar si el usuario ya existe
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario);
            
            if (usuarioExistente)
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso");
            }
            
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            
            usuario.FechaCreacion = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            
            // Generar dieta si no es administrador
            if (!usuario.EsAdministrador)
            {
                await _dietaService.GenerarDietaSemanalAsync(usuario.Id);
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var esAdmin = HttpContext.Session.GetString("EsAdministrador") == "True";
            
            if (usuarioId == null || !esAdmin)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null || usuario.EsAdministrador)
            {
                return NotFound();
            }
            
            return View(usuario);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }
            
            // Validar fecha de vencimiento
            if (usuario.FechaVencimientoDieta.Date <= DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaVencimientoDieta", "La fecha de vencimiento debe ser superior a la fecha actual");
            }
            
            // Verificar si el usuario ya existe (excepto el actual)
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario && u.Id != id);
            
            if (usuarioExistente)
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso");
            }
            
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            
            try
            {
                var usuarioOriginal = await _context.Usuarios.FindAsync(id);
                if (usuarioOriginal != null)
                {
                    usuarioOriginal.NombreUsuario = usuario.NombreUsuario;
                    usuarioOriginal.Contraseña = usuario.Contraseña;
                    usuarioOriginal.Email = usuario.Email;
                    usuarioOriginal.EsAdministrador = usuario.EsAdministrador;
                    usuarioOriginal.FechaVencimientoDieta = usuario.FechaVencimientoDieta;
                    
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.Id))
                {
                    return NotFound();
                }
                throw;
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AsignarDieta(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var recetas = await _context.Recetas.ToListAsync();

            var modelo = new AsignarDietaViewModel
            {
                UsuarioId = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Recetas = recetas
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarDieta(int UsuarioId, int[] DesayunoIds, int[] ComidaIds, int[] CenaIds)
        {
            // Busca la dieta semanal activa o crea una nueva
            var dietaSemanal = await _context.DietasSemanales
                .FirstOrDefaultAsync(ds => ds.UsuarioId == UsuarioId && ds.EsActiva);

            if (dietaSemanal == null)
            {
                dietaSemanal = new DietaSemanal
                {
                    UsuarioId = UsuarioId,
                    FechaInicio = DateTime.Today,
                    FechaFin = DateTime.Today.AddDays(6),
                    EsActiva = true
                };
                _context.DietasSemanales.Add(dietaSemanal);
                await _context.SaveChangesAsync();
            }

            // Elimina dietas diarias anteriores de esa semana
            var dietasDiarias = _context.DietasDiarias.Where(dd => dd.DietaSemanalId == dietaSemanal.Id);
            _context.DietasDiarias.RemoveRange(dietasDiarias);
            await _context.SaveChangesAsync();

            // Crea las nuevas dietas diarias
            for (int i = 0; i < 7; i++)
            {
                var dietaDiaria = new DietaDiaria
                {
                    DietaSemanalId = dietaSemanal.Id,
                    Fecha = DateTime.Today.AddDays(i),
                    RecetaDesayunoId = DesayunoIds.Length > i ? DesayunoIds[i] : null,
                    RecetaComidaId = ComidaIds.Length > i ? ComidaIds[i] : null,
                    RecetaCenaId = CenaIds.Length > i ? CenaIds[i] : null
                };
                _context.DietasDiarias.Add(dietaDiaria);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var esAdmin = HttpContext.Session.GetString("EsAdministrador") == "True";
            
            if (usuarioId == null || !esAdmin)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && !usuario.EsAdministrador)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index");
        }


        
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
