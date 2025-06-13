
using System.ComponentModel.DataAnnotations;

namespace AutodietaSemanal.Models
{
    public class DietaSemanal
    {
        public int Id { get; set; }
        
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        
        [Required]
        public DateTime FechaInicio { get; set; }
        
        [Required]
        public DateTime FechaFin { get; set; }
        
        public bool EsActiva { get; set; } = true;
        
        // Navegación
        public virtual ICollection<DietaDiaria> DietasDiarias { get; set; } = new List<DietaDiaria>();
    }
}
