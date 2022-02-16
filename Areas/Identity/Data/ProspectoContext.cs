using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prospecto.Areas.Identity.Data;

namespace Prospecto.Data;

public class ProspectoContext : IdentityDbContext<ProspectoUser>
{
    public ProspectoContext(DbContextOptions<ProspectoContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        //ModelBuilder.Entity<Rol>().HasData(
        //       new Rol { Id = 1, Nombre = "Administrador" },
        //       new Rol { Id = 2, Nombre = "Vendedor" },
        //       new Rol { Id = 3, Nombre = "Cliente" },
        //       new Rol { Id = 4, Nombre = "Supervisor" }
        //);
    }
    //public virtual DbSet<IdentityRole> Rol { get; set; }
}
