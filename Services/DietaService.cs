
using AutodietaSemanal.Data;
using AutodietaSemanal.Models;
using Microsoft.EntityFrameworkCore;

namespace AutodietaSemanal.Services
{
    public class DietaService
    {
        private readonly AutodietaContext _context;
        private readonly Random _random;
        
        public DietaService(AutodietaContext context)
        {
            _context = context;
            _random = new Random();
        }
        
        public async Task<DietaSemanal> GenerarDietaSemanalAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) throw new ArgumentException("Usuario no encontrado");
            
            // Desactivar dietas anteriores
            var dietasAnteriores = await _context.DietasSemanales
                .Where(d => d.UsuarioId == usuarioId && d.EsActiva)
                .ToListAsync();
            
            foreach (var dieta in dietasAnteriores)
            {
                dieta.EsActiva = false;
            }
            
            // Crear nueva dieta semanal
            var fechaInicio = DateTime.Today;
            var fechaFin = fechaInicio.AddDays(6);
            
            var dietaSemanal = new DietaSemanal
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                EsActiva = true
            };
            
            _context.DietasSemanales.Add(dietaSemanal);
            await _context.SaveChangesAsync();
            
            // Generar dietas diarias
            await GenerarDietasDiariasAsync(dietaSemanal.Id);
            
            return dietaSemanal;
        }
        
        private async Task GenerarDietasDiariasAsync(int dietaSemanalId)
        {
            var recetas = await _context.Recetas.ToListAsync();
            var desayunos = recetas.Where(r => r.TipoComida == TipoComida.Desayuno && !r.EsComidaLibre).ToList();
            var comidas = recetas.Where(r => r.TipoComida == TipoComida.Comida && !r.EsComidaLibre).ToList();
            var cenas = recetas.Where(r => r.TipoComida == TipoComida.Cena && !r.EsComidaLibre).ToList();
            var comidasLibres = recetas.Where(r => r.EsComidaLibre).ToList();
            
            var dietaSemanal = await _context.DietasSemanales.FindAsync(dietaSemanalId);
            
            Receta? ultimaComida = null;
            Receta? ultimaCena = null;
            
            for (int i = 0; i < 7; i++)
            {
                var fecha = dietaSemanal!.FechaInicio.AddDays(i);
                var esFinDeSemana = fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
                
                var dietaDiaria = new DietaDiaria
                {
                    DietaSemanalId = dietaSemanalId,
                    Fecha = fecha
                };
                
                // Asignar desayuno (nunca es comida libre)
                dietaDiaria.RecetaDesayunoId = desayunos[_random.Next(desayunos.Count)].Id;
                
                // Asignar comida
                if (esFinDeSemana && _random.NextDouble() < 0.4) // 40% probabilidad de comida libre en fin de semana
                {
                    var comidaLibre = comidasLibres.Where(c => c.TipoComida == TipoComida.Comida).ToList();
                    if (comidaLibre.Any())
                    {
                        dietaDiaria.RecetaComidaId = comidaLibre[_random.Next(comidaLibre.Count)].Id;
                    }
                    else
                    {
                        dietaDiaria.RecetaComidaId = ObtenerRecetaDiferente(comidas, ultimaComida)?.Id;
                    }
                }
                else
                {
                    dietaDiaria.RecetaComidaId = ObtenerRecetaDiferente(comidas, ultimaComida)?.Id;
                }
                
                // Asignar cena
                if (esFinDeSemana && _random.NextDouble() < 0.4) // 40% probabilidad de comida libre en fin de semana
                {
                    var cenaLibre = comidasLibres.Where(c => c.TipoComida == TipoComida.Cena).ToList();
                    if (cenaLibre.Any())
                    {
                        dietaDiaria.RecetaCenaId = cenaLibre[_random.Next(cenaLibre.Count)].Id;
                    }
                    else
                    {
                        dietaDiaria.RecetaCenaId = ObtenerRecetaDiferente(cenas, ultimaCena)?.Id;
                    }
                }
                else
                {
                    dietaDiaria.RecetaCenaId = ObtenerRecetaDiferente(cenas, ultimaCena)?.Id;
                }
                
                // Actualizar últimas recetas para evitar repetición
                ultimaComida = await _context.Recetas.FindAsync(dietaDiaria.RecetaComidaId);
                ultimaCena = await _context.Recetas.FindAsync(dietaDiaria.RecetaCenaId);
                
                _context.DietasDiarias.Add(dietaDiaria);
            }
            
            await _context.SaveChangesAsync();
        }
        
        private Receta? ObtenerRecetaDiferente(List<Receta> recetas, Receta? ultimaReceta)
        {
            if (recetas.Count <= 1) return recetas.FirstOrDefault();
            
            var recetasDisponibles = ultimaReceta != null 
                ? recetas.Where(r => r.Id != ultimaReceta.Id).ToList()
                : recetas;
                
            return recetasDisponibles.Any() 
                ? recetasDisponibles[_random.Next(recetasDisponibles.Count)]
                : recetas.FirstOrDefault();
        }
    }
}
