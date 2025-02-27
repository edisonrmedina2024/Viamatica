using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<RolOpcione> RolOpciones { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-S935ES7\\SQLEXPRESS01;Initial Catalog=DBADMIN;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__Persona__A47881417DC6E4E0");

            entity.ToTable("Persona");

            entity.HasIndex(e => e.Identificacion, "UQ__Persona__D6F931E50F33F952").IsUnique();

            entity.Property(e => e.IdPersona).HasColumnName("idPersona");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.FechaNacimiento).HasColumnType("date");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolOpcione>(entity =>
        {
            entity.HasKey(e => e.IdOpcion).HasName("PK__RolOpcio__A914DF3576D26144");

            entity.Property(e => e.IdOpcion).HasColumnName("idOpcion");
            entity.Property(e => e.NombreOpcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__3C872F76ECBEAEA2");

            entity.HasIndex(e => e.RolName, "UQ__Roles__12BAAF0406C45641").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.RolName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.IdUsuarios).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolUsuario",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolUsuarios_Usuarios"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolUsuarios_Rol"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdUsuario").HasName("PK__rol_usua__4AC25D4C9AD724C1");
                        j.ToTable("rol_usuarios");
                        j.IndexerProperty<int>("IdRol").HasColumnName("idRol");
                        j.IndexerProperty<int>("IdUsuario").HasColumnName("idUsuario");
                    });

            entity.HasMany(d => d.RolOpcionesIdOpcions).WithMany(p => p.RolIdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolRolOpcione",
                    r => r.HasOne<RolOpcione>().WithMany()
                        .HasForeignKey("RolOpcionesIdOpcion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolRolOpciones_Opciones"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RolIdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolRolOpciones_Rol"),
                    j =>
                    {
                        j.HasKey("RolIdRol", "RolOpcionesIdOpcion").HasName("PK__rol_rolO__6AC66B43D496728A");
                        j.ToTable("rol_rolOpciones");
                        j.IndexerProperty<int>("RolIdRol").HasColumnName("Rol_idRol");
                        j.IndexerProperty<int>("RolOpcionesIdOpcion").HasColumnName("RolOpciones_idOpcion");
                    });
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.IdSession).HasName("PK__Sessions__ADE2668F890A8553");

            entity.Property(e => e.IdSession).HasColumnName("idSession");
            entity.Property(e => e.FechaCierre).HasColumnType("datetime");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UsuariosIdUsuario).HasColumnName("usuarios_idUsuario");

            entity.HasOne(d => d.UsuariosIdUsuarioNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UsuariosIdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__645723A66FC5F395");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Mail)
                .HasMaxLength(120)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PersonaIdPersona).HasColumnName("Persona_idPersona");
            entity.Property(e => e.SessionActive)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("('N')")
                .IsFixedLength();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('Activo')")
                .IsFixedLength();
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PersonaIdPersonaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.PersonaIdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Persona");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
