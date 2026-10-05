using DimDim.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Data;

public class DimDimContext(DbContextOptions<DimDimContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Conta> Contas => Set<Conta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().HasIndex(c => c.Cpf).IsUnique();
        modelBuilder.Entity<Conta>().HasIndex(c => new { c.Agencia, c.Numero }).IsUnique();

        // Uma conta pertence a um cliente; apagar o cliente apaga as contas dele (ON DELETE CASCADE no DDL)
        modelBuilder.Entity<Conta>()
            .HasOne(c => c.Cliente)
            .WithMany(c => c.Contas)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
