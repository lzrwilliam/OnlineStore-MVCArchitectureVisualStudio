using Microsoft.EntityFrameworkCore;
using ProiectPAW.Models;
using ProiectPAW.wwwroot.Models;
using ProiectPAWMvc.Models;

namespace ProiectPAW.ContextModels
{
    public class ProdusContext : DbContext
    {
        public DbSet<Produs> Produs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Utilizator> Utilizator { get; set; }
        public DbSet<Recenzie> Recenzie { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<ProdusVizualizat> ProdusVizualizat { get; set; }
        public DbSet<Notificare> Notificari { get; set; }
        public DbSet<CosCumparaturi> CosCumparaturi { get; set; }
        public DbSet<AlertaPret> AlertaPret { get; set; }

        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<DetaliiComanda> DetaliiComenzi { get; set; }
        public DbSet<AtributCategorie> AtributCategorii { get; set; }
        public DbSet<ValoareAtribut> ValoareAtribute { get; set; }

        public ProdusContext(DbContextOptions<ProdusContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Subcategorii)
                .WithOne(c => c.ParentCategorie)
                .HasForeignKey(c => c.ParentCategorieID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Produse)
                .WithOne(p => p.Categorie)
                .HasForeignKey(p => p.CategorieID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Atribute)
                .WithOne(a => a.Categorie)
                .HasForeignKey(a => a.CategorieID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Produs>()
                .HasMany(p => p.ValoriAtribute)
                .WithOne(va => va.Produs)
                .HasForeignKey(va => va.ProdusID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AtributCategorie>()
                .HasMany(ac => ac.ValoriAtribute)
                .WithOne(va => va.AtributCategorie)
                .HasForeignKey(va => va.AtributCategorieID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Produs>()
                  .ToTable("Produs")
                  .HasMany(p => p.Recenzii)
                  .WithOne(r => r.Produs)
                  .HasForeignKey(r => r.ProdusID)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recenzie>()
                .ToTable("Recenzie");

            modelBuilder.Entity<Produs>()
                 .ToTable("Produs");


            modelBuilder.Entity<DetaliiComanda>()
                    .HasOne(dc => dc.Produs)
                    .WithMany(p => p.Comenzi)
                    .HasForeignKey(dc => dc.ProdusID)
                    .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetaliiComanda>()
                .HasOne(dc => dc.Comanda)
                .WithMany(c => c.DetaliiComenzi)
                .HasForeignKey(dc => dc.ComandaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recenzie>()
                .HasOne(r => r.Utilizator)
                .WithMany()
                .HasForeignKey(r => r.UtilizatorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificare>()
                .HasOne(n => n.Utilizator)
                .WithMany()
                .HasForeignKey(n => n.UtilizatorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificare>()
                .HasOne(n => n.Produs)
                .WithMany()
                .HasForeignKey(n => n.ProdusID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProdusVizualizat>().ToTable("ProdusVizualizat");

            modelBuilder.Entity<Produs>()
                   .HasMany(p => p.Comenzi)
                   .WithOne(dc => dc.Produs)
                   .HasForeignKey(dc => dc.ProdusID)
                   .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Produs>()
                .HasMany(p => p.Favorite)
                .WithOne(f => f.Produs)
                .HasForeignKey(f => f.ProdusID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
