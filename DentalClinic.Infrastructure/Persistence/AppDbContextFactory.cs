using DentalClinic.Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DentalClinic.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DentalClinic.DesignTime;Integrated Security=True;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }
}
