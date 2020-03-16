using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyAPI
{
    public partial class sfsContext : DbContext
    {
        public sfsContext()
        {
        }

        public sfsContext(DbContextOptions<sfsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<FilesProp> FilesProp { get; set; }
        public virtual DbSet<NotCompressionExtentions> NotCompressionExtentions { get; set; }
        public virtual DbSet<RSystem> RSystem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Files>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileData).IsRequired();

                entity.Property(e => e.IsCompress)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<FilesProp>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.Property(e => e.FileId).ValueGeneratedNever();

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Prop1).HasMaxLength(500);

                entity.Property(e => e.Prop10).HasMaxLength(500);

                entity.Property(e => e.Prop2).HasMaxLength(500);

                entity.Property(e => e.Prop3).HasMaxLength(500);

                entity.Property(e => e.Prop4).HasMaxLength(500);

                entity.Property(e => e.Prop5).HasMaxLength(500);

                entity.Property(e => e.Prop6).HasMaxLength(500);

                entity.Property(e => e.Prop7).HasMaxLength(500);

                entity.Property(e => e.Prop8).HasMaxLength(500);

                entity.Property(e => e.Prop9).HasMaxLength(500);

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<NotCompressionExtentions>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Extention)
                    .IsRequired()
                    .HasColumnName("extention")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RSystem>(entity =>
            {
                entity.HasKey(e => e.SystemId);

                entity.ToTable("r_System");

                entity.Property(e => e.SystemId).ValueGeneratedNever();

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
