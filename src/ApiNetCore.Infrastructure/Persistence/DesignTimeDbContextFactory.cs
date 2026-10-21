using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApiNetCore.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
     Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
     ?? "Data Source=IT-PC1\\MSSQLIT1;Initial Catalog=ApiNetCoreDb;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
