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

            // Configurar relaciones con Recetas
            modelBuilder.Entity<DietaDiaria>()
                .HasOne(d => d.RecetaDesayuno)
                .WithMany()
                .HasForeignKey(d => d.RecetaDesayunoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DietaDiaria>()
                .HasOne(d => d.RecetaComida)
                .WithMany()
                .HasForeignKey(d => d.RecetaComidaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DietaDiaria>()
                .HasOne(d => d.RecetaCena)
                .WithMany()
                .HasForeignKey(d => d.RecetaCenaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Crear usuario administrador por defecto - FECHAS FIJAS
            var fechaCreacion = new DateTime(2024, 1, 1);
            var fechaVencimiento = new DateTime(2025, 12, 31);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    Contraseña = "admin123",
                    Email = "admin@autodieta.com",
                    EsAdministrador = true,
                    FechaVencimientoDieta = fechaVencimiento,
                    FechaCreacion = fechaCreacion
                }
            );

            // Crear recetas de ejemplo
            var recetas = new[]
            {
                // Desayunos
                new Receta { Id = 1, Nombre = "Tostadas con Aguacate", Descripcion = "Desayuno saludable con aguacate", Ingredientes = "2 rebanadas de pan integral, 1 aguacate, sal, pimienta", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 2, Nombre = "Avena con Frutas", Descripcion = "Avena nutritiva con frutas frescas", Ingredientes = "1 taza de avena, leche, plátano, fresas, miel", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 3, Nombre = "Huevos Revueltos", Descripcion = "Huevos revueltos con verduras", Ingredientes = "3 huevos, cebolla, tomate, pimiento, aceite", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 4, Nombre = "Yogurt con Granola", Descripcion = "Yogurt natural con granola casera", Ingredientes = "1 taza de yogurt natural, granola, miel, nueces", TipoComida = TipoComida.Desayuno },
                new Receta { Id = 5, Nombre = "Smoothie Verde", Descripcion = "Batido verde energético", Ingredientes = "Espinacas, plátano, manzana, agua, limón", TipoComida = TipoComida.Desayuno },
                
                // Comidas
                new Receta { Id = 6, Nombre = "Ensalada César", Descripcion = "Ensalada fresca con pollo", Ingredientes = "Lechuga, pollo, croutones, queso parmesano, aderezo césar", TipoComida = TipoComida.Comida },
                new Receta { Id = 7, Nombre = "Pasta con Verduras", Descripcion = "Pasta integral con verduras al vapor", Ingredientes = "Pasta integral, brócoli, zanahoria, calabacín, aceite de oliva", TipoComida = TipoComida.Comida },
                new Receta { Id = 8, Nombre = "Pollo a la Plancha", Descripcion = "Pechuga de pollo con ensalada", Ingredientes = "Pechuga de pollo, lechuga, tomate, pepino, aceite de oliva", TipoComida = TipoComida.Comida },
                new Receta { Id = 9, Nombre = "Arroz con Pollo", Descripcion = "Arroz integral con pollo y verduras", Ingredientes = "Arroz integral, pollo, pimientos, guisantes, caldo", TipoComida = TipoComida.Comida },
                new Receta { Id = 10, Nombre = "Quinoa con Verduras", Descripcion = "Quinoa nutritiva con verduras asadas", Ingredientes = "Quinoa, calabacín, berenjena, pimiento, aceite de oliva", TipoComida = TipoComida.Comida },
                new Receta { Id = 11, Nombre = "Pizza Libre", Descripcion = "Pizza de fin de semana", Ingredientes = "Masa de pizza, queso, jamón, champiñones", TipoComida = TipoComida.Comida, EsComidaLibre = true },
                
                // Cenas
                new Receta { Id = 12, Nombre = "Sopa de Verduras", Descripcion = "Sopa ligera de verduras", Ingredientes = "Caldo de verduras, zanahoria, apio, cebolla, calabacín", TipoComida = TipoComida.Cena },
                new Receta { Id = 13, Nombre = "Pescado al Horno", Descripcion = "Pescado con verduras al horno", Ingredientes = "Filete de pescado, patatas, cebolla, limón, hierbas", TipoComida = TipoComida.Cena },
                new Receta { Id = 14, Nombre = "Tortilla de Patatas", Descripcion = "Tortilla española tradicional", Ingredientes = "Huevos, patatas, cebolla, aceite, sal", TipoComida = TipoComida.Cena },
                new Receta { Id = 15, Nombre = "Ensalada de Atún", Descripcion = "Ensalada ligera con atún", Ingredientes = "Lechuga, atún, tomate, pepino, aceitunas, aceite", TipoComida = TipoComida.Cena },
                new Receta { Id = 16, Nombre = "Crema de Calabaza", Descripcion = "Crema suave de calabaza", Ingredientes = "Calabaza, cebolla, caldo, nata, especias", TipoComida = TipoComida.Cena },
                new Receta { Id = 17, Nombre = "Hamburguesa Libre", Descripcion = "Hamburguesa de fin de semana", Ingredientes = "Pan de hamburguesa, carne, queso, lechuga, tomate", TipoComida = TipoComida.Cena, EsComidaLibre = true }
            };

            modelBuilder.Entity<Receta>().HasData(recetas);
        }
    }
}

