using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.Models;

namespace StorkDorkMain.Data;

public partial class StorkDorkContext : DbContext
{
    public StorkDorkContext()
    {
    }

    public StorkDorkContext(DbContextOptions<StorkDorkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bird> Birds { get; set; }

    public virtual DbSet<Checklist> Checklists { get; set; }

    public virtual DbSet<ChecklistItem> ChecklistItems { get; set; }

    public virtual DbSet<SdUser> SdUsers { get; set; }

    public virtual DbSet<Sighting> Sightings { get; set; }

    public virtual DbSet<Milestone> Milestone { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:StorkDorkDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bird__3214EC27B23D066D");

            entity.ToTable("Bird");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ScientificName).HasMaxLength(100);
            entity.Property(e => e.CommonName).HasMaxLength(100);
            entity.Property(e => e.SpeciesCode).HasMaxLength(10);
            entity.Property(e => e.Category).HasMaxLength(10);
            entity.Property(e => e.Order).HasMaxLength(25);
            entity.Property(e => e.FamilyCommonName).HasMaxLength(50);
            entity.Property(e => e.FamilyScientificName).HasMaxLength(50);
            entity.Property(e => e.ReportAs).HasMaxLength(10);
            entity.Property(e => e.Range).HasMaxLength(1000);
        });

        modelBuilder.Entity<Checklist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Checklis__3214EC271BFFB94F");

            entity.ToTable("Checklist");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChecklistName).HasMaxLength(100);
            entity.Property(e => e.SdUserId).HasColumnName("SDUserID");

            // entity.HasOne(d => d.SdUser).WithMany(p => p.Checklists)
            //     .HasForeignKey(d => d.SdUserId)
            //     .HasConstraintName("FK_Checklist_SDUser");
        });

        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Checklis__3214EC27AEE131D7");

            entity.ToTable("ChecklistItem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BirdId).HasColumnName("BirdID");
            entity.Property(e => e.ChecklistId).HasColumnName("ChecklistID");

            entity.HasOne(d => d.Bird).WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.BirdId)
                .HasConstraintName("FK_ChecklistItem_Bird");

            entity.HasOne(d => d.Checklist).WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.ChecklistId)
                .HasConstraintName("FK_ChecklistItem_Checklist");
        });

        modelBuilder.Entity<SdUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SDUser__3214EC277D9B2DC9");

            entity.ToTable("SDUser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AspNetIdentityId)
                .HasMaxLength(450)
                .HasColumnName("AspNetIdentityID");
        });

        modelBuilder.Entity<Sighting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sighting__3214EC2734C618BE");

            entity.ToTable("Sighting");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BirdId).HasColumnName("BirdID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Notes).HasMaxLength(3000);
            entity.Property(e => e.SdUserId).HasColumnName("SDUserID");

            entity.HasOne(d => d.Bird).WithMany(p => p.Sightings)
                .HasForeignKey(d => d.BirdId)
                .HasConstraintName("FK_Sighting_Bird");

            // entity.HasOne(d => d.SdUser).WithMany(p => p.Sightings)
            //     .HasForeignKey(d => d.SdUserId)
            //     .HasConstraintName("FK_Sighting_SDUser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
