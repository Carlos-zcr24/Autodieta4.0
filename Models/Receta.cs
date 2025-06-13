
using System.ComponentModel.DataAnnotations;

namespace AutodietaSemanal.Models
{
    public class Receta
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de la receta es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Los ingredientes son requeridos")]
        [StringLength(2000, ErrorMessage = "Los ingredientes no pueden exceder 2000 caracteres")]
        public string Ingredientes { get; set; } = string.Empty;
        
        public TipoComida TipoComida { get; set; }
        
        public bool EsComidaLibre { get; set; } = false;
        
        // Navegación
        public virtual ICollection<DietaDiaria> DietasDiarias { get; set; } = new List<DietaDiaria>();
    }
    
    public enum TipoComida
    {
        Desayuno = 1,
        Comida = 2,
        Cena = 3
    }
}
