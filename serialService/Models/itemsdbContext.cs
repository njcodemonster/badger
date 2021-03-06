﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace itemService_entity.Models
{
    public partial class itemsdbContext : DbContext
    {
        public itemsdbContext()
        {
        }

        public itemsdbContext(DbContextOptions<itemsdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EventTypes> EventTypes { get; set; }
        public virtual DbSet<ItemEvents> ItemEvents { get; set; }
        public virtual DbSet<ItemStatus> ItemStatus { get; set; }
        public virtual DbSet<Items> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("Server=itemsdb.cl35upw5sngr.us-west-1.rds.amazonaws.com;database=itemsdb;uid=admin;pwd=Captain2018.;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<EventTypes>(entity =>
            {
                entity.HasKey(e => e.EventTypeId);

                entity.ToTable("event_types", "itemsdb");

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EventTypeDescription)
                    .HasColumnName("event_type_description")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeName)
                    .HasColumnName("event_type_name")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ItemEvents>(entity =>
            {
                entity.HasKey(e => e.ItemEventId);

                entity.ToTable("item_events", "itemsdb");

                entity.HasIndex(e => e.EventTypeId)
                    .HasName("event_type_id_idx");

                entity.HasIndex(e => e.ItemId)
                    .HasName("item_id_idx");

                entity.Property(e => e.ItemEventId)
                    .HasColumnName("item_event_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Barcode).HasColumnName("barcode");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.EventNotes)
                    .HasColumnName("event_notes")
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReferenceId)
                    .HasColumnName("reference_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.EventType)
                    .WithMany(p => p.ItemEvents)
                    .HasForeignKey(d => d.EventTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("event_type_id");
            });

            modelBuilder.Entity<ItemStatus>(entity =>
            {
                entity.ToTable("item_status", "itemsdb");

                entity.Property(e => e.ItemStatusId)
                    .HasColumnName("item_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.Barcode });

                entity.ToTable("items", "itemsdb");

                entity.HasIndex(e => e.ItemStatusId)
                    .HasName("item_status_id_idx");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Barcode)
                    .HasColumnName("barcode")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.BagCode)
                    .HasColumnName("bag_code")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ItemStatusId)
                    .HasColumnName("item_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Published)
                    .HasColumnName("published")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PublishedBy)
                    .HasColumnName("published_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RaStatus)
                    .HasColumnName("ra_status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SkuFamily)
                    .IsRequired()
                    .HasColumnName("sku_family")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SkuId)
                    .HasColumnName("sku_id")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.SlotNumber)
                    .HasColumnName("slot_number")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ItemStatus)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("item_status_id");
            });
        }
    }
}
