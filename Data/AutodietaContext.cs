
using Microsoft.EntityFrameworkCore;
using AutodietaSemanal.Models;

namespace AutodietaSemanal.Data
{
    public class AutodietaContext : DbContext
    {
        public AutodietaContext(DbContextOptions<AutodietaContext> options) : base(options)
        {
        }
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<DietaSemanal> DietasSemanales { get; set; }
        public DbSet<DietaDiaria> DietasDiarias { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurar relaciones
            modelBuilder.Entity<DietaSemanal>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Dietas)
                .HasForeignKey(d => d.UsuarioId);
                
            modelBuilder.Entity<DietaDiaria>()
                .HasOne(d => d.DietaSemanal)
                .WithMany(ds => ds.DietasDiarias)
                .HasForeignKey(d => d.DietaSemanalId);
                
            // Seed data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Crear usuario administrador por defecto
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    Contraseña = "admin123",
                    Email = "admin@autodieta.com",
                    EsAdministrador = true,
                    FechaVencimientoDieta = DateTime.Now.AddDays(365),
                    FechaCreacion = DateTime.Now
                }
            );
            
            // Crear recetas de ejemplo
            var recetas = new[]
            {
                // Desayunos
                new Receta { Id = 1, Nombre = "Tostadas con Aguacate", Descripcion = "Desayuno saludable con aguacate", Ingredientes = "2 rebanadas de pan integral, 1 aguacate, sal, pimienta", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 2, Nombre = "Avena con Frutas", Descripcion = "Avena nutritiva con frutas frescas", Ingredientes = "1 taza de avena, leche, plátano, fresas, miel", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 3, Nombre = "Huevos Revueltos", Descripcion = "Huevos revueltos con verduras", Ingredientes = "3 huevos, cebolla, tomate, pimiento, aceite", TipoComida = TipoComida.Desayuno },
                
                // Comidas
                new Receta { Id = 4, Nombre = "Ensalada César", Descripcion = "Ensalada fresca con pollo", Ingredientes = "Lechuga, pollo, croutones, queso parmesano, aderezo césar", TipoComida = TipoComida.Comida },
                new Receta { Id = 5, Nombre = "Pasta con Verduras", Descripción = "Pasta integral con verduras al vapor", Ingredientes = "Pasta integral, brócoli, zanahoria, calabacín, aceite de oliva", TipoComida = TipoComida.Comida },
                new Receta { Id = 6, Nombre = "Pollo a la Plancha", Descripcion = "Pechuga de pollo con ensalada", Ingredientes = "Pechuga de pollo, lechuga, tomate, pepino, aceite de oliva", TipoComida = TipoComida.Comida },
                new Receta { Id = 7, Nombre = "Pizza Libre", Descripcion = "Pizza de fin de semana", Ingredientes = "Masa de pizza, queso, jamón, champiñones", TipoComida = TipoComida.Comida, EsComidaLibre = true },
                
                // Cenas
                new Receta { Id = 8, Nombre = "Sopa de Verduras", Descripcion = "Sopa ligera de verduras", Ingredientes = "Caldo de verduras, zanahoria, apio, cebolla, calabacín", TipoComida = TipoComida.Cena },
                new Receta { Id = 9, Nombre = "Pescado al Horno", Descripcion = "Pescado con verduras al horno", Ingredientes = "Filete de pescado, patatas, cebolla, limón, hierbas", TipoComida = TipoComida.Cena },
                new Receta { Id = 10, Nombre = "Tortilla de Patatas", Descripcion = "Tortilla española tradicional", Ingredientes = "Huevos, patatas, cebolla, aceite, sal", TipoComida = TipoComida.Cena },
                new Receta { Id = 11, Nombre = "Hamburguesa Libre", Descripcion = "Hamburguesa de fin de semana", Ingredientes = "Pan de hamburguesa, carne, queso, lechuga, tomate", TipoComida = TipoComida.Cena, EsComidaLibre = true }
            };
            
            modelBuilder.Entity<Receta>().HasData(recetas);
        }
    }
}
