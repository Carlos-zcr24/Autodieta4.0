
using System.ComponentModel.DataAnnotations;

namespace AutodietaSemanal.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Contraseña { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;
        
        public bool EsAdministrador { get; set; } = false;
        
        [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
        [Display(Name = "Fecha de Vencimiento de Dieta")]
        public DateTime FechaVencimientoDieta { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual ICollection<DietaSemanal> Dietas { get; set; } = new List<DietaSemanal>();
    }
}
