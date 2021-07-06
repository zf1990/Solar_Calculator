using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Persistence.Models
{
    public partial class Energy_projectContext : DbContext
    {
        public Energy_projectContext()
        {
        }

        public Energy_projectContext(DbContextOptions<Energy_projectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AllSkySolarRadAvg> AllSkySolarRadAvgs { get; set; }
        public virtual DbSet<AllSkySolarRadStd> AllSkySolarRadStds { get; set; }
        public virtual DbSet<TauB> TauBs { get; set; }
        public virtual DbSet<TauD> TauDs { get; set; }
        public virtual DbSet<WeatherStation> WeatherStations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-2A8QUTV\\ZHOUINSTANCE;Database=Energy_project;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AllSkySolarRadAvg>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AllSkySolarRadAvg");
            });

            modelBuilder.Entity<AllSkySolarRadStd>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AllSkySolarRadStd");
            });

            modelBuilder.Entity<TauB>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TauB");

                entity.Property(e => e.TauBapr21).HasColumnName("TauBApr21");

                entity.Property(e => e.TauBaug21).HasColumnName("TauBAug21");

                entity.Property(e => e.TauBdec21)
                    .HasMaxLength(255)
                    .HasColumnName("TauBDec21");

                entity.Property(e => e.TauBfeb21).HasColumnName("TauBFeb21");

                entity.Property(e => e.TauBjan21).HasColumnName("TauBJan21");

                entity.Property(e => e.TauBjul21).HasColumnName("TauBJul21");

                entity.Property(e => e.TauBjun21).HasColumnName("TauBJun21");

                entity.Property(e => e.TauBmar21).HasColumnName("TauBMar21");

                entity.Property(e => e.TauBmay21).HasColumnName("TauBMay21");

                entity.Property(e => e.TauBnov21).HasColumnName("TauBNov21");

                entity.Property(e => e.TauBoct21).HasColumnName("TauBOct21");

                entity.Property(e => e.TauBsep21).HasColumnName("TauBSep21");
            });

            modelBuilder.Entity<TauD>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TauD");

                entity.Property(e => e.TauDapr21).HasColumnName("TauDApr21");

                entity.Property(e => e.TauDaug21).HasColumnName("TauDAug21");

                entity.Property(e => e.TauDdec21)
                    .HasMaxLength(255)
                    .HasColumnName("TauDDec21");

                entity.Property(e => e.TauDfeb21).HasColumnName("TauDFeb21");

                entity.Property(e => e.TauDjan21).HasColumnName("TauDJan21");

                entity.Property(e => e.TauDjul21).HasColumnName("TauDJul21");

                entity.Property(e => e.TauDjun21).HasColumnName("TauDJun21");

                entity.Property(e => e.TauDmar21).HasColumnName("TauDMar21");

                entity.Property(e => e.TauDmay21).HasColumnName("TauDMay21");

                entity.Property(e => e.TauDnov21).HasColumnName("TauDNov21");

                entity.Property(e => e.TauDoct21).HasColumnName("TauDOct21");

                entity.Property(e => e.TauDsep21).HasColumnName("TauDSep21");
            });

            modelBuilder.Entity<WeatherStation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("WeatherStation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
