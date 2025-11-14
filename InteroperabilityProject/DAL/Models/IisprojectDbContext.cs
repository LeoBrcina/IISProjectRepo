using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class IisprojectDbContext : DbContext
{
    public IisprojectDbContext()
    {
    }

    public IisprojectDbContext(DbContextOptions<IisprojectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<ProfilePicture> ProfilePictures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("PK__AppUser__EAE6D9DFA01BBABA");

            entity.ToTable("AppUser");

            entity.HasIndex(e => e.Username, "UQ__AppUser__536C85E4E75FC5B6").IsUnique();

            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(200);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profiles__3214EC07781D328F");

            entity.HasIndex(e => e.PublicIdentifier, "UQ__Profiles__C2E1251BC2A85937").IsUnique();

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PublicIdentifier).HasMaxLength(200);
            entity.Property(e => e.Subtitle).HasMaxLength(500);
            entity.Property(e => e.SubtitleV2).HasMaxLength(500);
            entity.Property(e => e.TextActionTarget).HasMaxLength(500);
            entity.Property(e => e.TitleV2).HasMaxLength(255);
        });

        modelBuilder.Entity<ProfilePicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProfileP__3214EC07DF0F0001");

            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.Profile).WithMany(p => p.ProfilePictures)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK_ProfilePictures_Profiles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
