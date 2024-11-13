using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Entities
{
    public partial class EmployeeDBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Constructor with IConfiguration injection
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Retrieve the connection string from appsettings.json
                var connectionString = _configuration.GetConnectionString("EmployeeDbConnection");
                optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId).HasName("PK__Employee__AF2DBB991F6A4C55");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DateOfJoining).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
