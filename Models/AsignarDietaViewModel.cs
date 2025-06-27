using AutodietaSemanal.Models;
using System.ComponentModel.DataAnnotations;

namespace AutodietaSemanal.Models
{
    public class AsignarDietaViewModel
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public List<Receta> Recetas { get; set; }

        // Para cada día, selecciona una receta para desayuno, comida y cena
        public List<int?> DesayunoIds { get; set; } = new();
        public List<int?> ComidaIds { get; set; } = new();
        public List<int?> CenaIds { get; set; } = new();
    }
}
