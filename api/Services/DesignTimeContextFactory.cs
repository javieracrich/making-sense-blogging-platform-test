using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Services
{
	public class DesignTimeContextFactory : IDesignTimeDbContextFactory<MakingSenseDbContext>
	{
		public MakingSenseDbContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
		   .SetBasePath(Directory.GetCurrentDirectory())
		   .AddJsonFile("appsettings.json")
		   .Build();

			var builder = new DbContextOptionsBuilder<MakingSenseDbContext>();

			builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

			return new MakingSenseDbContext(builder.Options);
		}
	}
}
