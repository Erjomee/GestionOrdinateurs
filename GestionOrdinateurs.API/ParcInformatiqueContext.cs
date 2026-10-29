using GestionOrdinateurs.Shared;

using Microsoft.EntityFrameworkCore;
public class ParcInformatiqueContext : DbContext
{
   public DbSet<Ordinateur> Ordinateurs { get; set; }
   public DbSet<Reparation> Reparations { get; set; }
   public DbSet<User> Users { get; set; }

   public ParcInformatiqueContext(
        DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Stocke les caractéristiques dans la table Ordinateur
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        // Un ordinateur peut avoir plusieurs réparations
        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur)
            .HasForeignKey(r => r.UnOrdinateurId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
    }
}