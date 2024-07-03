using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RepairPlatform.Entities;

public partial class Repairguy20118046Context : IdentityDbContext<AspNetUsers>
{
    public Repairguy20118046Context()
    {
    }

    public Repairguy20118046Context(DbContextOptions<Repairguy20118046Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Log20118046> Log20118046s { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<Repairguy> Repairguys { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseCollation("Cyrillic_General_CI_AI");

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdministratorId).HasName("PK__Administ__ACDEFE330C04B382");

            entity.ToTable("Administrator", "20118046");

            entity.HasIndex(e => e.Aemail, "UQ__Administ__19B023C1C5FBADD4").IsUnique();

            entity.Property(e => e.AdministratorId).HasColumnName("AdministratorID");
            entity.Property(e => e.Aemail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("AEmail");
            entity.Property(e => e.AfullName)
                .HasMaxLength(100)
                .HasColumnName("AFullName");
            entity.Property(e => e.Apassword).HasColumnName("APassword");
            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Client__E67E1A243F8EF71C");

            entity.ToTable("Client", "20118046", tb => tb.HasTrigger("trg_Client_InsertUpdate"));

            entity.HasOne(r => r.User)
            .WithOne(u => u.Clients)
            .HasForeignKey<Client>(r => r.UserId);

            entity.Property(e => e.Cemail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CEmail");
            entity.Property(e => e.CfirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CFirstName");
            entity.Property(e => e.ClastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CLastName");
            entity.Property(e => e.Cpassword)
                .HasMaxLength(255)
                .HasColumnName("CPassword");
            entity.Property(e => e.Cphoto).HasColumnName("CPhoto");
            entity.Property(e => e.Cstatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("('Active')")
                .HasColumnName("CStatus");
            entity.Property(e => e.Ctelephone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CTelephone");
            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Group__149AF36A11118AAE");

            entity.ToTable("Group", "20118046", tb => tb.HasTrigger("trg_Group_InsertUpdate"));

            entity.Property(e => e.CatDescription).HasColumnType("text");
            entity.Property(e => e.CatName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
        });

        modelBuilder.Entity<Log20118046>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__log_2011__5E5499A86972F052");

            entity.ToTable("log_20118046", "20118046");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.AdministratorId).HasColumnName("AdministratorID");
            entity.Property(e => e.OperationDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.TableName).HasMaxLength(100);
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => e.RepairId).HasName("PK__Repair__07D0BC2D045EA68E");

            entity.ToTable("Repair", "20118046", tb => tb.HasTrigger("trg_Repair_InsertUpdate"));

            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
            entity.Property(e => e.RepDescription).HasColumnType("text");
            entity.Property(e => e.RepName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasMany(d => d.Groups).WithMany(p => p.Repairs)
                .UsingEntity<Dictionary<string, object>>(
                    "RepairGroup",
                    r => r.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RepairGroups_Group"),
                    l => l.HasOne<Repair>().WithMany()
                        .HasForeignKey("RepairId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RepairGroups_Repair"),
                    j =>
                    {
                        j.HasKey("RepairId", "GroupId").HasName("PK__RepairGr__B699131BD65873E8");
                        j.ToTable("RepairGroups", "20118046", tb => tb.HasTrigger("trg_RepairGroups_InsertUpdate"));
                    });
        });

        modelBuilder.Entity<Repairguy>(entity =>
        {
            entity.HasKey(e => e.RepairguyId).HasName("PK__Repairgu__B8B2482240CB3143");

            entity.ToTable("Repairguy", "20118046", tb => tb.HasTrigger("trg_Repairguy_InsertUpdate"));

            entity.HasOne(r => r.User)
            .WithOne(u => u.Repairguys)
            .HasForeignKey<Repairguy>(r => r.UserId);

            entity.HasOne(d => d.Town).WithMany(p => p.Repairguys)
              .HasForeignKey(d => d.TownId)
              .HasConstraintName("FK_Repairguy_Town");

            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
            entity.Property(e => e.Rdescription)
                .HasColumnType("text")
                .HasColumnName("RDescription");
            entity.Property(e => e.Remail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("REmail");
            entity.Property(e => e.RfirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RFirstName");
            entity.Property(e => e.RlastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RLastName");
            entity.Property(e => e.Rpassword)
                .HasMaxLength(255)
                .HasColumnName("RPassword");
            entity.Property(e => e.Rphoto).HasColumnName("RPhoto");
            entity.Property(e => e.Rstatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("('Active')")
                .HasColumnName("RStatus");
            entity.Property(e => e.Rtelephone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RTelephone");

            entity.HasMany(d => d.Repairs).WithMany(p => p.Repairguys)
                .UsingEntity<Dictionary<string, object>>(
                    "RepairguyRepair",
                    r => r.HasOne<Repair>().WithMany()
                        .HasForeignKey("RepairId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RepairguyRepairs_Repair"),
                    l => l.HasOne<Repairguy>().WithMany()
                        .HasForeignKey("RepairguyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RepairguyRepairs_Repairguy"),
                    j =>
                    {
                        j.HasKey("RepairguyId", "RepairId").HasName("PK__Repairgu__78CF43E0703FB982");
                        j.ToTable("RepairguyRepairs", "20118046", tb => tb.HasTrigger("trg_RepairguyRepairs_InsertUpdate"));
                    });


        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F242316F357");

            entity.ToTable("Reservation", "20118046", tb => tb.HasTrigger("trg_Reservation_InsertUpdate"));

            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
            entity.Property(e => e.ResComment).HasColumnType("text");
            entity.Property(e => e.ResDateTime).HasColumnType("datetime");
            entity.Property(e => e.ResLocation)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Client");

            entity.HasOne(d => d.Group).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Group");

            entity.HasOne(d => d.Repairguy).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RepairguyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Repairguy");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79CEE547ABA7");

            entity.ToTable("Review", "20118046", tb => tb.HasTrigger("trg_Review_InsertUpdate"));

            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
            entity.Property(e => e.RevComment).HasColumnType("text");
            entity.Property(e => e.RevDateTime).HasColumnType("datetime");
            entity.Property(e => e.RevLocation)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Client");

            entity.HasOne(d => d.Group).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Group");

            entity.HasOne(d => d.Repairguy).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RepairguyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Repairguy");

            // Add this line to define the relationship
            entity.HasOne(d => d.Reservation)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Reservation");
        });

        modelBuilder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Town");

            entity.ToTable("Town", "20118046", tb => tb.HasTrigger("trg_Town_InsertUpdate"));

            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.Property(e => e.LastModified20118046)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("LastModified_20118046");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
