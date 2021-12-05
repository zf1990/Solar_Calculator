using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Persistence.Models;

#nullable disable

namespace Persistence
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WeatherDatum> WeatherData { get; set; }
        public virtual DbSet<WeatherStation> WeatherStations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-2A8QUTV\\ZHOUINSTANCE; Initial Catalog=SolarCalculator; Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<WeatherDatum>(entity =>
            {
                entity.HasKey(e => new { e.WeatherStationId, e.Date });

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.WeatherStation)
                    .WithMany(p => p.WeatherData)
                    .HasForeignKey(d => d.WeatherStationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherData_WeahterStation");
            });

            modelBuilder.Entity<WeatherStation>(entity =>
            {
                entity.HasIndex(e => new { e.Longitude, e.Latitude }, "IX_COORDINATES");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
