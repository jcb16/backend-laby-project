using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public partial class UniversityContext : DbContext
{
    public UniversityContext()
    {
    }

    public UniversityContext(DbContextOptions<UniversityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<RankingCriterion> RankingCriteria { get; set; }

    public virtual DbSet<RankingSystem> RankingSystems { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    public virtual DbSet<UniversityRankingYear> UniversityRankingYears { get; set; }

    public virtual DbSet<UniversityYear> UniversityYears { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=University;Username=postgres;Password=Start123++");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_country");

            entity.ToTable("country", "universities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("country_name");
        });

        modelBuilder.Entity<RankingCriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_rc");

            entity.ToTable("ranking_criteria", "universities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("criteria_name");
            entity.Property(e => e.RankingSystemId).HasColumnName("ranking_system_id");

            entity.HasOne(d => d.RankingSystem).WithMany(p => p.RankingCriteria)
                .HasForeignKey(d => d.RankingSystemId)
                .HasConstraintName("fk_rc_rs");
        });

        modelBuilder.Entity<RankingSystem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_rs");

            entity.ToTable("ranking_system", "universities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SystemName)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("system_name");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_uni");

            entity.ToTable("university", "universities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.UniversityName)
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("university_name");

            entity.HasOne(d => d.Country).WithMany(p => p.Universities)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_uni_cnt");
        });

        modelBuilder.Entity<UniversityRankingYear>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("university_ranking_year", "universities");

            entity.Property(e => e.RankingCriteriaId).HasColumnName("ranking_criteria_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UniversityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("university_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.RankingCriteria).WithMany()
                .HasForeignKey(d => d.RankingCriteriaId)
                .HasConstraintName("fk_ury_rc");

            entity.HasOne(d => d.University).WithMany()
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ury_uni");
        });

        modelBuilder.Entity<UniversityYear>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("university_year", "universities");

            entity.Property(e => e.NumStudents).HasColumnName("num_students");
            entity.Property(e => e.PctFemaleStudents).HasColumnName("pct_female_students");
            entity.Property(e => e.PctInternationalStudents).HasColumnName("pct_international_students");
            entity.Property(e => e.StudentStaffRatio)
                .HasPrecision(6, 2)
                .HasDefaultValueSql("NULL::numeric")
                .HasColumnName("student_staff_ratio");
            entity.Property(e => e.UniversityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("university_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.University).WithMany()
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uy_uni");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
