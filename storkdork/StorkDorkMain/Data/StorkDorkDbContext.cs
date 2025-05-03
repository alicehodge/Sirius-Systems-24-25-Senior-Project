using Microsoft.EntityFrameworkCore;
using StorkDorkMain.Models;

namespace StorkDorkMain.Data;

public class StorkDorkDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public StorkDorkDbContext(DbContextOptions<StorkDorkDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    // Entity Sets
    public virtual DbSet<Bird> Birds { get; set; }
    public virtual DbSet<Checklist> Checklists { get; set; }
    public virtual DbSet<ChecklistItem> ChecklistItems { get; set; }
    public virtual DbSet<SdUser> SdUsers { get; set; }
    public virtual DbSet<Sighting> Sightings { get; set; }
    public virtual DbSet<Milestone> Milestones { get; set; }
    public virtual DbSet<ModeratedContent> ModeratedContent { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<Milestone>? Milestone { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bird>(entity =>
        {
            entity.ToTable("Bird");
            entity.HasKey(e => e.Id).HasName("PK__Bird__3214EC27152FA685");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CommonName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ScientificName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Order).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FamilyCommonName).HasMaxLength(100);
            entity.Property(e => e.FamilyScientificName).HasMaxLength(100);
            entity.Property(e => e.SpeciesCode).HasMaxLength(10);
            entity.Property(e => e.Category).HasMaxLength(10);
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

            entity.HasOne(d => d.SdUser).WithMany(p => p.Sightings)
                .HasForeignKey(d => d.SdUserId)
                .HasConstraintName("FK_Sighting_SDUser");
        });

        modelBuilder.Entity<UserSettings>(entity => 
        {
            entity.HasKey(e => e.Id).HasName("PK_UserSettings_3214EC27");
            entity.Property(e => e.AnonymousSightings).HasDefaultValue(false);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");
            entity.HasKey(e => e.Id).HasName("PK_Notifications");
            
            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);
                
            entity.Property(e => e.RelatedUrl)
                .HasMaxLength(200);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Milestone__3214EC27");
            
            entity.Property(e => e.SDUserId)
                .IsRequired()
                .HasColumnName("SDUserID");
            entity.Property(e => e.SightingsMade)
                .IsRequired()
                .HasDefaultValue(0);
            entity.Property(e => e.PhotosContributed)
                .IsRequired()
                .HasDefaultValue(0);
        });

        modelBuilder.Entity<ModeratedContent>(entity =>
        {
            entity.ToTable("ModeratedContent");
            entity.HasKey(e => e.Id);
            
            // Configure discriminator for TPH inheritance
            entity.HasDiscriminator<string>("ContentType")
                .HasValue<ModeratedContent>("Base")
                .HasValue<RangeSubmission>("BirdRange");
            
            // Configure properties
            entity.Property(e => e.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.SubmittedDate)
                .IsRequired();
            
            entity.Property(e => e.SubmitterId)
                .IsRequired();
            
            entity.Property(e => e.ModeratorId)
                .IsRequired(false);
            
            entity.Property(e => e.ModeratorNotes)
                .IsRequired(false);

            entity.Property(e => e.ModeratedDate)
                .IsRequired(false);
            
            // Navigation properties
            entity.HasOne(e => e.Submitter)
                .WithMany()
                .HasForeignKey(e => e.SubmitterId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Moderator)
                .WithMany()
                .HasForeignKey(e => e.ModeratorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
