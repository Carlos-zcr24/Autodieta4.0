using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Data;
using AutodietaSemanal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutodietaSemanal.Controllers
{
    public class RecetasController : Controller
    {
        private readonly AutodietaContext _context;

        public RecetasController(AutodietaContext context)
        {
            _context = context;
        }

        // GET: Recetas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Recetas.ToListAsync());
        }

        // GET: Recetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var receta = await _context.Recetas.FirstOrDefaultAsync(m => m.Id == id);
            if (receta == null) return NotFound();

            return View(receta);
        }

        // GET: Recetas/Create
        public IActionResult Create()
        {
            ViewData["TipoComida"] = new SelectList(Enum.GetValues(typeof(TipoComida)));
            return View();
        }

        // POST: Recetas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Ingredientes,TipoComida,EsComidaLibre")] Receta receta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoComida"] = new SelectList(Enum.GetValues(typeof(TipoComida)));
            return View(receta);
        }

        // GET: Recetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var receta = await _context.Recetas.FindAsync(id);
            if (receta == null) return NotFound();

            ViewData["TipoComida"] = new SelectList(Enum.GetValues(typeof(TipoComida)), receta.TipoComida);
            return View(receta);
        }

        // POST: Recetas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Ingredientes,TipoComida,EsComidaLibre")] Receta receta)
        {
            if (id != receta.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecetaExists(receta.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoComida"] = new SelectList(Enum.GetValues(typeof(TipoComida)), receta.TipoComida);
            return View(receta);
        }

        // GET: Recetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var receta = await _context.Recetas.FirstOrDefaultAsync(m => m.Id == id);
            if (receta == null) return NotFound();

            return View(receta);
        }

        // POST: Recetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);
            if (receta != null)
            {
                _context.Recetas.Remove(receta);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RecetaExists(int id)
        {
            return _context.Recetas.Any(e => e.Id == id);
        }
    }
}