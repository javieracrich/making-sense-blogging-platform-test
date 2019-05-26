using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Services
{
    public class MakingSenseDbContext : IdentityDbContext<User>
    {
        public MakingSenseDbContext(DbContextOptions<MakingSenseDbContext> options) : base(options)
        {

        }

        public DbSet<BlogPost> BlogPost { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            SetDefaultMaxLength(builder, 500);
        }

        private static void SetDefaultMaxLength(ModelBuilder builder, int maxLength)
        {
            var properties = builder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() == null && !p.Name.Contains("Id") && !p.DeclaringType.Name.Contains("Microsoft"))
                .ToList();

            foreach (var p in properties)
            {
                p.SetMaxLength(maxLength);
                p.IsUnicode(false);
            }
        }
    }
}
