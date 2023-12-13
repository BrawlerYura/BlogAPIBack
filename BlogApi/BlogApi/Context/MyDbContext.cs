using System;
using System.Collections.Generic;
using BlogApi.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsAddrObj> AsAddrObjs { get; set; }

    public virtual DbSet<AsAdmHierarchy> AsAdmHierarchies { get; set; }

    public virtual DbSet<AsHouse> AsHouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost;port=5432;database=blogdb;username=postgres;password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsAddrObj>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("as_addr_obj");

            entity.Property(e => e.Changeid).HasColumnName("changeid");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Isactual).HasColumnName("isactual");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Nextid).HasColumnName("nextid");
            entity.Property(e => e.Objectguid).HasColumnName("objectguid");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.Opertypeid).HasColumnName("opertypeid");
            entity.Property(e => e.Previd).HasColumnName("previd");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Typename).HasColumnName("typename");
            entity.Property(e => e.Updatedate).HasColumnName("updatedate");
        });

        modelBuilder.Entity<AsAdmHierarchy>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("as_adm_hierarchy");

            entity.Property(e => e.Areacode).HasColumnName("areacode");
            entity.Property(e => e.Changeid).HasColumnName("changeid");
            entity.Property(e => e.Citycode).HasColumnName("citycode");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Nextid).HasColumnName("nextid");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.Parentobjid).HasColumnName("parentobjid");
            entity.Property(e => e.Path).HasColumnName("path");
            entity.Property(e => e.Placecode).HasColumnName("placecode");
            entity.Property(e => e.Plancode).HasColumnName("plancode");
            entity.Property(e => e.Previd).HasColumnName("previd");
            entity.Property(e => e.Regioncode).HasColumnName("regioncode");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Streetcode).HasColumnName("streetcode");
            entity.Property(e => e.Updatedate).HasColumnName("updatedate");
        });

        modelBuilder.Entity<AsHouse>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("as_houses");

            entity.Property(e => e.Addnum1).HasColumnName("addnum1");
            entity.Property(e => e.Addnum2).HasColumnName("addnum2");
            entity.Property(e => e.Addtype1).HasColumnName("addtype1");
            entity.Property(e => e.Addtype2).HasColumnName("addtype2");
            entity.Property(e => e.Changeid).HasColumnName("changeid");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Housenum).HasColumnName("housenum");
            entity.Property(e => e.Housetype).HasColumnName("housetype");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Isactual).HasColumnName("isactual");
            entity.Property(e => e.Nextid).HasColumnName("nextid");
            entity.Property(e => e.Objectguid).HasColumnName("objectguid");
            entity.Property(e => e.Objectid).HasColumnName("objectid");
            entity.Property(e => e.Opertypeid).HasColumnName("opertypeid");
            entity.Property(e => e.Previd).HasColumnName("previd");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Updatedate).HasColumnName("updatedate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
