using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.Models;

namespace StorkDorkMain.Models;

public partial class StorkDorkDbContext : DbContext
{
    public StorkDorkDbContext()
    {
    }

    public StorkDorkDbContext(DbContextOptions<StorkDorkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bird> Birds { get; set; }

    public virtual DbSet<Checklist> Checklists { get; set; }

    public virtual DbSet<ChecklistItem> ChecklistItems { get; set; }

    public virtual DbSet<SdUser> SdUsers { get; set; }

    public virtual DbSet<Sighting> Sightings { get; set; }

    public virtual DbSet<UserSettings> UserSettings {get;set;}

    public virtual DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bird__3214EC27152FA685");
        });

        modelBuilder.Entity<Checklist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Checklis__3214EC2791C5CD06");

            // entity.HasOne(d => d.SdUser).WithMany(p => p.Checklists).HasConstraintName("FK_Checklist_SDUser");
        });

        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Checklis__3214EC27391E9E0F");

            entity.HasOne(d => d.Bird).WithMany(p => p.ChecklistItems).HasConstraintName("FK_ChecklistItem_Bird");

            entity.HasOne(d => d.Checklist).WithMany(p => p.ChecklistItems).HasConstraintName("FK_ChecklistItem_Checklist");
        });

        modelBuilder.Entity<SdUser>(entity =>
        {
            entity.ToTable("SDUser");

            entity.HasKey(e => e.Id).HasName("PK__SDUser__3214EC27D4DAB424");
        });

        modelBuilder.Entity<Sighting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sighting__3214EC27BEAC21ED");

            entity.HasOne(d => d.Bird).WithMany(p => p.Sightings).HasConstraintName("FK_Sighting_Bird");

            // entity.HasOne(d => d.SdUser).WithMany(p => p.Sightings).HasConstraintName("FK_Sighting_SDUser");
        });

        modelBuilder.Entity<UserSettings>(entity => 
        {
            entity.HasKey(e => e.Id).HasName("PK_UserSettings_3214EC27");

            entity.Property(e => e.AnonymousSightings).HasDefaultValue(false);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notification__3214EC27D4DAB424");

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.RelatedUrl)
                .HasMaxLength(200);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notification_SDUser");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
