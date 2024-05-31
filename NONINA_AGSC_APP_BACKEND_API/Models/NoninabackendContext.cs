using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class NoninabackendContext : DbContext
{
    public NoninabackendContext()
    {
    }

    public NoninabackendContext(DbContextOptions<NoninabackendContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agenda> Agendas { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5AKSQUN;Database=NONINABACKEND;TrustServerCertificate=true;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agenda>(entity =>
        {
            entity.HasOne(d => d.Evento).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.EventoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agendas_Eventos");

            entity.HasOne(d => d.User).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agendas_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.MunicipalityId).HasColumnName("Municipality_id");
            entity.Property(e => e.PlaceCoordinates)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.PlaceLabel)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.Category).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_Categories");

            entity.HasOne(d => d.Municipality).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.MunicipalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_Municipalities");

            entity.HasOne(d => d.User).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_Users");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsFixedLength();

            entity.HasOne(d => d.Publication).WithMany(p => p.Messages)
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Publications");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users");
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsFixedLength();

            entity.HasOne(d => d.Evento).WithMany(p => p.Publications)
                .HasForeignKey(d => d.EventoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Publications_Eventos");

            entity.HasOne(d => d.User).WithMany(p => p.Publications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Publications_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
