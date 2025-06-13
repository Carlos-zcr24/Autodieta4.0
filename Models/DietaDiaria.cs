
using System.ComponentModel.DataAnnotations;

namespace AutodietaSemanal.Models
{
    public class DietaDiaria
    {
        public int Id { get; set; }
        
        public int DietaSemanalId { get; set; }
        public virtual DietaSemanal DietaSemanal { get; set; } = null!;
        
        [Required]
        public DateTime Fecha { get; set; }
        
        public DayOfWeek DiaSemana => Fecha.DayOfWeek;
        
        // Recetas del día
        public int? RecetaDesayunoId { get; set; }
        public virtual Receta? RecetaDesayuno { get; set; }
        
        public int? RecetaComidaId { get; set; }
        public virtual Receta? RecetaComida { get; set; }
        
        public int? RecetaCenaId { get; set; }
        public virtual Receta? RecetaCena { get; set; }
    }
}
