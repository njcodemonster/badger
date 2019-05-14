using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace badgerApi.Models
{
    public partial class productdbContext : DbContext
    {
        public productdbContext()
        {
        }

        public productdbContext(DbContextOptions<productdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttributeType> AttributeType { get; set; }
        public virtual DbSet<AttributeValues> AttributeValues { get; set; }
        public virtual DbSet<Attributes> Attributes { get; set; }
        public virtual DbSet<Calculations> Calculations { get; set; }
        public virtual DbSet<CalculationsExtra> CalculationsExtra { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<CategoryOptions> CategoryOptions { get; set; }
        public virtual DbSet<PhotoshootEvents> PhotoshootEvents { get; set; }
        public virtual DbSet<PhotoshootModels> PhotoshootModels { get; set; }
        public virtual DbSet<Photoshoots> Photoshoots { get; set; }
        public virtual DbSet<PoClaim> PoClaim { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttributeValues> ProductAttributeValues { get; set; }
        public virtual DbSet<ProductAttributes> ProductAttributes { get; set; }
        public virtual DbSet<ProductCategories> ProductCategories { get; set; }
        public virtual DbSet<ProductEvents> ProductEvents { get; set; }
        public virtual DbSet<ProductPageDetailTypes> ProductPageDetailTypes { get; set; }
        public virtual DbSet<ProductPageDetails> ProductPageDetails { get; set; }
        public virtual DbSet<ProductPhotoshoots> ProductPhotoshoots { get; set; }
        public virtual DbSet<ProductShootStatus> ProductShootStatus { get; set; }
        public virtual DbSet<ProductSizeAndFit> ProductSizeAndFit { get; set; }
        public virtual DbSet<ProductTypes> ProductTypes { get; set; }
        public virtual DbSet<ProductUsedIn> ProductUsedIn { get; set; }
        public virtual DbSet<ProductWashTypes> ProductWashTypes { get; set; }
        public virtual DbSet<PurchaseOrderDiscounts> PurchaseOrderDiscounts { get; set; }
        public virtual DbSet<PurchaseOrderEvents> PurchaseOrderEvents { get; set; }
        public virtual DbSet<PurchaseOrderLedger> PurchaseOrderLedger { get; set; }
        public virtual DbSet<PurchaseOrderLineItems> PurchaseOrderLineItems { get; set; }
        public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatus { get; set; }
        public virtual DbSet<PurchaseOrderTracking> PurchaseOrderTracking { get; set; }
        public virtual DbSet<PurchaseOrders> PurchaseOrders { get; set; }
        public virtual DbSet<RaStatus> RaStatus { get; set; }
        public virtual DbSet<SizeAndFitOtherText> SizeAndFitOtherText { get; set; }
        public virtual DbSet<Sku> Sku { get; set; }
        public virtual DbSet<TransactionTypes> TransactionTypes { get; set; }
        public virtual DbSet<UserAccessLevels> UserAccessLevels { get; set; }
        public virtual DbSet<UserEvents> UserEvents { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VendorAddress> VendorAddress { get; set; }
        public virtual DbSet<VendorContactPerson> VendorContactPerson { get; set; }
        public virtual DbSet<VendorProducts> VendorProducts { get; set; }
        public virtual DbSet<VendorTypes> VendorTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("Server=itemsdb.cl35upw5sngr.us-west-1.rds.amazonaws.com;database=productdb;uid=admin;pwd=Captain2018.;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AttributeType>(entity =>
            {
                entity.ToTable("attribute_type", "productdb");

                entity.Property(e => e.AttributeTypeId)
                    .HasColumnName("attribute_type_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeTypeDescription)
                    .IsRequired()
                    .HasColumnName("attribute_type_description")
                    .IsUnicode(false);

                entity.Property(e => e.AttributeTypeName)
                    .IsRequired()
                    .HasColumnName("attribute_type_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<AttributeValues>(entity =>
            {
                entity.HasKey(e => e.ValueId);

                entity.ToTable("attribute_values", "productdb");

                entity.Property(e => e.ValueId)
                    .HasColumnName("value_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Attributes>(entity =>
            {
                entity.HasKey(e => e.AttributeId);

                entity.ToTable("attributes", "productdb");

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Attribute)
                    .HasColumnName("attribute")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AttributeDisplayName)
                    .HasColumnName("attribute_display_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AttributeTypeId)
                    .HasColumnName("attribute_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DataType)
                    .IsRequired()
                    .HasColumnName("data_type")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("varchar");
            });

            modelBuilder.Entity<Calculations>(entity =>
            {
                entity.HasKey(e => e.CalculationId);

                entity.ToTable("calculations", "productdb");

                entity.Property(e => e.CalculationId)
                    .HasColumnName("calculation_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CalculationName)
                    .IsRequired()
                    .HasColumnName("calculation_name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("decimal(10,0)");
            });

            modelBuilder.Entity<CalculationsExtra>(entity =>
            {
                entity.HasKey(e => e.CalculationId);

                entity.ToTable("calculations_extra", "productdb");

                entity.Property(e => e.CalculationId)
                    .HasColumnName("calculation_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.ExtraName)
                    .IsRequired()
                    .HasColumnName("extra_name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExtraValue)
                    .IsRequired()
                    .HasColumnName("extra_value")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.ToTable("categories", "productdb");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnName("category_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryParentId)
                    .HasColumnName("category_parent_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryType)
                    .HasColumnName("category_type")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<CategoryOptions>(entity =>
            {
                entity.HasKey(e => e.CategoryOptionId);

                entity.ToTable("category_options", "productdb");

                entity.HasIndex(e => e.AttributeId)
                    .HasName("attribute_id(this uses attributes table along with products)");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("category_id");

                entity.Property(e => e.CategoryOptionId)
                    .HasColumnName("category_option_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("double(10,0)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("double(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PhotoshootEvents>(entity =>
            {
                entity.HasKey(e => e.PhotoshootEventId);

                entity.ToTable("photoshoot_events", "productdb");

                entity.Property(e => e.PhotoshootEventId)
                    .HasColumnName("photoshoot_event_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.EventNotes)
                    .IsRequired()
                    .HasColumnName("event_notes")
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoshootId)
                    .HasColumnName("photoshoot_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReferenceId)
                    .HasColumnName("reference_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PhotoshootModels>(entity =>
            {
                entity.HasKey(e => e.ModelId);

                entity.ToTable("photoshoot_models", "productdb");

                entity.Property(e => e.ModelId)
                    .HasColumnName("model_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActiveStatus)
                    .HasColumnName("active_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModelEthnicity)
                    .HasColumnName("model_ethnicity")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModelHair)
                    .HasColumnName("model_hair")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModelHeight)
                    .HasColumnName("model_height")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModelName)
                    .IsRequired()
                    .HasColumnName("model_name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Photoshoots>(entity =>
            {
                entity.HasKey(e => e.PhotoshootId);

                entity.ToTable("photoshoots", "productdb");

                entity.Property(e => e.PhotoshootId)
                    .HasColumnName("photoshoot_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActiveStatus)
                    .HasColumnName("active_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModelId)
                    .HasColumnName("model_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoshootName)
                    .IsRequired()
                    .HasColumnName("photoshoot_name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ShootEndDate).HasColumnName("shoot_end_date");

                entity.Property(e => e.ShootStartDate).HasColumnName("shoot_start_date");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PoClaim>(entity =>
            {
                entity.HasKey(e => e.PoId);

                entity.ToTable("po_claim", "productdb");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.InspectClaimedAt)
                    .HasColumnName("inspect_claimed_at")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.InspectClaimer)
                    .HasColumnName("inspect_claimer")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoClaimcol)
                    .HasColumnName("po_claimcol")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.PublishClaimedAt)
                    .HasColumnName("publish_claimed_at")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.PublishClaimer)
                    .HasColumnName("publish_claimer")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product", "productdb");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsOnSiteStatus)
                    .HasColumnName("is_on_site_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductAvailability)
                    .HasColumnName("product_availability")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.ProductCost)
                    .HasColumnName("product_cost")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.ProductDescription)
                    .IsRequired()
                    .HasColumnName("product_description")
                    .HasColumnType("longtext");

                entity.Property(e => e.ProductDiscount)
                    .HasColumnName("product_discount")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductRetail)
                    .HasColumnName("product_retail")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.ProductTypeId)
                    .HasColumnName("product_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductUrlHandle)
                    .IsRequired()
                    .HasColumnName("product_url_handle")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductVendorImage)
                    .IsRequired()
                    .HasColumnName("product_vendor_image")
                    .IsUnicode(false);

                entity.Property(e => e.PublishedAt).HasColumnName("published_at");

                entity.Property(e => e.PublishedStatus)
                    .HasColumnName("published_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SizeAndFitId)
                    .HasColumnName("size_and_fit_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SkuFamily)
                    .IsRequired()
                    .HasColumnName("sku_family")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorColorName)
                    .IsRequired()
                    .HasColumnName("vendor_color_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WashTypeId)
                    .HasColumnName("wash_type_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductAttributeValues>(entity =>
            {
                entity.HasKey(e => e.ProductAttributeValueId);

                entity.ToTable("product_attribute_values", "productdb");

                entity.Property(e => e.ProductAttributeValueId)
                    .HasColumnName("product_attribute_value_id")
                    .HasColumnType("bigint(200)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ValueId)
                    .HasColumnName("value_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductAttributes>(entity =>
            {
                entity.HasKey(e => e.ProductAttributeId);

                entity.ToTable("product_attributes", "productdb");

                entity.Property(e => e.ProductAttributeId)
                    .HasColumnName("product_attribute_id")
                    .HasColumnType("bigint(200)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sku)
                    .HasColumnName("sku")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.HasKey(e => e.ProductCategoryId);

                entity.ToTable("product_categories", "productdb");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("category_id");

                entity.HasIndex(e => e.ProductId)
                    .HasName("product_id");

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("product_category_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductEvents>(entity =>
            {
                entity.HasKey(e => e.ProductEventId);

                entity.ToTable("product_events", "productdb");

                entity.HasIndex(e => e.ReferenceId)
                    .HasName("reference_id");

                entity.Property(e => e.ProductEventId)
                    .HasColumnName("product_event_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.EventNotes)
                    .IsRequired()
                    .HasColumnName("event_notes")
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReferenceId)
                    .HasColumnName("reference_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductPageDetailTypes>(entity =>
            {
                entity.HasKey(e => e.ProductDetailTypeId);

                entity.ToTable("product_page_detail_types", "productdb");

                entity.HasIndex(e => e.ProductDetailTypeId)
                    .HasName("pd_type_id");

                entity.Property(e => e.ProductDetailTypeId)
                    .HasColumnName("product_detail_type_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductDetailDescription)
                    .IsRequired()
                    .HasColumnName("product_detail_description")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ProductDetailName)
                    .IsRequired()
                    .HasColumnName("product_detail_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductPageDetails>(entity =>
            {
                entity.HasKey(e => e.ProductPageDetailId);

                entity.ToTable("product_page_details", "productdb");

                entity.Property(e => e.ProductPageDetailId)
                    .HasColumnName("product_page_detail_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductDetailType)
                    .HasColumnName("product_detail_type")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductDetailValue)
                    .IsRequired()
                    .HasColumnName("product_detail_value")
                    .IsUnicode(false);

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductPhotoshoots>(entity =>
            {
                entity.HasKey(e => new { e.PhotoshootId, e.ProductId });

                entity.ToTable("product_photoshoots", "productdb");

                entity.Property(e => e.PhotoshootId)
                    .HasColumnName("photoshoot_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductShootStatusId)
                    .HasColumnName("product_shoot_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductShootStatus>(entity =>
            {
                entity.ToTable("product_shoot_status", "productdb");

                entity.Property(e => e.ProductShootStatusId)
                    .HasColumnName("product_shoot_status_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ProductShootDescription)
                    .HasColumnName("product_shoot_description")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductShootStatusName)
                    .HasColumnName("product_shoot_status_name")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductSizeAndFit>(entity =>
            {
                entity.HasKey(e => e.SizeAndFitId);

                entity.ToTable("product_size_and_fit", "productdb");

                entity.Property(e => e.SizeAndFitId)
                    .HasColumnName("size_and_fit_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.SizeAndFitName)
                    .IsRequired()
                    .HasColumnName("size_and_fit_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductTypes>(entity =>
            {
                entity.HasKey(e => e.ProductTypeId);

                entity.ToTable("product_types", "productdb");

                entity.Property(e => e.ProductTypeId)
                    .HasColumnName("product_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductType)
                    .IsRequired()
                    .HasColumnName("product_type")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductUsedIn>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.PoId });

                entity.ToTable("product_used_in", "productdb");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<ProductWashTypes>(entity =>
            {
                entity.HasKey(e => e.WashTypeId);

                entity.ToTable("product_wash_types", "productdb");

                entity.Property(e => e.WashTypeId)
                    .HasColumnName("wash_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WashType)
                    .IsRequired()
                    .HasColumnName("wash_type")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurchaseOrderDiscounts>(entity =>
            {
                entity.HasKey(e => e.PoDiscountId);

                entity.ToTable("purchase_order_discounts", "productdb");

                entity.Property(e => e.PoDiscountId)
                    .HasColumnName("po_discount_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompletedStatus)
                    .HasColumnName("completed_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DiscountNote)
                    .IsRequired()
                    .HasColumnName("discount_note")
                    .IsUnicode(false);

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnName("discount_percentage")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrderEvents>(entity =>
            {
                entity.HasKey(e => e.PoEventId);

                entity.ToTable("purchase_order_events", "productdb");

                entity.HasIndex(e => e.ReferenceId)
                    .HasName("reference_id");

                entity.Property(e => e.PoEventId)
                    .HasColumnName("po_event_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.EventNotes)
                    .IsRequired()
                    .HasColumnName("event_notes")
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReferenceId)
                    .HasColumnName("reference_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrderLedger>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.ToTable("purchase_order_ledger", "productdb");

                entity.HasIndex(e => e.PoId)
                    .HasName("po_id");

                entity.HasIndex(e => e.TransactionId)
                    .HasName("transaction_id");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Credit)
                    .HasColumnName("credit")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Debit)
                    .HasColumnName("debit")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrderLineItems>(entity =>
            {
                entity.HasKey(e => e.LineItemId);

                entity.ToTable("purchase_order_line_items", "productdb");

                entity.HasIndex(e => e.PoId)
                    .HasName("order_id");

                entity.HasIndex(e => e.ProductId)
                    .HasName("lineitem_product_id");

                entity.HasIndex(e => e.Sku)
                    .HasName("lineitem_sku");

                entity.HasIndex(e => e.VendorId)
                    .HasName("vendor_id");

                entity.Property(e => e.LineItemId)
                    .HasColumnName("line_item_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LineItemAcceptedQuantity)
                    .HasColumnName("line_item_accepted_quantity")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LineItemCost)
                    .HasColumnName("line_item_cost")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.LineItemOrderedQuantity)
                    .HasColumnName("line_item_ordered_quantity")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LineItemRejectedQuantity)
                    .HasColumnName("line_item_rejected_quantity")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LineItemRetail)
                    .HasColumnName("line_item_retail")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.LineItemType)
                    .HasColumnName("line_item_type")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrderStatus>(entity =>
            {
                entity.HasKey(e => e.PoStatusId);

                entity.ToTable("purchase_order_status", "productdb");

                entity.Property(e => e.PoStatusId)
                    .HasColumnName("po_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoStatusDescription)
                    .HasColumnName("po_status_description")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoStatusName)
                    .HasColumnName("po_status_name")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrderTracking>(entity =>
            {
                entity.HasKey(e => e.PoTrackingId);

                entity.ToTable("purchase_order_tracking", "productdb");

                entity.HasIndex(e => e.PoId)
                    .HasName("po_id");

                entity.HasIndex(e => e.TrackingNumber)
                    .HasName("tracking_number");

                entity.Property(e => e.PoTrackingId)
                    .HasColumnName("po_tracking_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TrackingNumber)
                    .HasColumnName("tracking_number")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PurchaseOrders>(entity =>
            {
                entity.HasKey(e => e.PoId);

                entity.ToTable("purchase_orders", "productdb");

                entity.HasIndex(e => e.PoId)
                    .HasName("po_id");

                entity.Property(e => e.PoId)
                    .HasColumnName("po_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Defected)
                    .HasColumnName("defected")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Deleted)
                    .HasColumnName("deleted")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeliveryWindowEnd).HasColumnName("delivery_window_end");

                entity.Property(e => e.DeliveryWindowStart).HasColumnName("delivery_window_start");

                entity.Property(e => e.GoodCondition)
                    .HasColumnName("good_condition")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OrderDate).HasColumnName("order_date");

                entity.Property(e => e.PoDiscountId)
                    .HasColumnName("po_discount_id")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.PoStatus)
                    .HasColumnName("po_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Shipping)
                    .HasColumnName("shipping")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Subtotal)
                    .HasColumnName("subtotal")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.TotalQuantity)
                    .HasColumnName("total_quantity")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorInvoiceNumber)
                    .IsRequired()
                    .HasColumnName("vendor_invoice_number")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.VendorOrderNumber)
                    .HasColumnName("vendor_order_number")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorPoNumber)
                    .IsRequired()
                    .HasColumnName("vendor_po_number")
                    .HasMaxLength(222)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RaStatus>(entity =>
            {
                entity.ToTable("ra_status", "productdb");

                entity.Property(e => e.RaStatusId)
                    .HasColumnName("ra_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RaStatusDescription)
                    .IsRequired()
                    .HasColumnName("ra_status_description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RaStatusName)
                    .IsRequired()
                    .HasColumnName("ra_status_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SizeAndFitOtherText>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.ToTable("size_and_fit_other_text", "productdb");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SizeAndFitOtherText1)
                    .IsRequired()
                    .HasColumnName("size_and_fit_other_text")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Sku>(entity =>
            {
                entity.ToTable("sku", "productdb");

                entity.HasIndex(e => e.Sku1)
                    .HasName("sku");

                entity.Property(e => e.SkuId)
                    .HasColumnName("sku_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sku1)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<TransactionTypes>(entity =>
            {
                entity.HasKey(e => e.TransactionTypeId);

                entity.ToTable("transaction_types", "productdb");

                entity.Property(e => e.TransactionTypeId)
                    .HasColumnName("transaction_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TransactionDescription)
                    .IsRequired()
                    .HasColumnName("transaction_description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionName)
                    .IsRequired()
                    .HasColumnName("transaction_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<UserAccessLevels>(entity =>
            {
                entity.HasKey(e => e.AccessLevelId);

                entity.ToTable("user_access_levels", "productdb");

                entity.Property(e => e.AccessLevelId)
                    .HasColumnName("access_level_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AccessLevel)
                    .IsRequired()
                    .HasColumnName("access_level")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserEvents>(entity =>
            {
                entity.HasKey(e => e.UserEventId);

                entity.ToTable("user_events", "productdb");

                entity.Property(e => e.UserEventId)
                    .HasColumnName("user_event_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EventDescription)
                    .HasColumnName("event_description")
                    .IsUnicode(false);

                entity.Property(e => e.ReferenceId)
                    .HasColumnName("reference_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserEventType)
                    .HasColumnName("user_event_type")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("users", "productdb");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AccessLevelId)
                    .HasColumnName("access_level_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActiveStatus)
                    .HasColumnName("active_status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Designation)
                    .HasColumnName("designation")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin).HasColumnName("last_login");

                entity.Property(e => e.LastSession).HasColumnName("last_session");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("vendor", "productdb");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActiveStatus)
                    .HasColumnName("active_status")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CorpName)
                    .HasColumnName("corp_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OurCustomerNumber)
                    .HasColumnName("our_customer_number")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.StatementName)
                    .HasColumnName("statement_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorCode)
                    .HasColumnName("vendor_code")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorDescription)
                    .IsRequired()
                    .HasColumnName("vendor_description")
                    .IsUnicode(false);

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasColumnName("vendor_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorSince).HasColumnName("vendor_since");

                entity.Property(e => e.VendorType)
                    .HasColumnName("vendor_type")
                    .HasColumnType("int(2)");
            });

            modelBuilder.Entity<VendorAddress>(entity =>
            {
                entity.ToTable("vendor_address", "productdb");

                entity.HasIndex(e => e.VendorId)
                    .HasName("vendor_id");

                entity.Property(e => e.VendorAddressId)
                    .HasColumnName("vendor_address_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorCity)
                    .IsRequired()
                    .HasColumnName("vendor_city")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorState)
                    .IsRequired()
                    .HasColumnName("vendor_state")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.VendorStreet)
                    .IsRequired()
                    .HasColumnName("vendor_street")
                    .IsUnicode(false);

                entity.Property(e => e.VendorSuiteNumber)
                    .IsRequired()
                    .HasColumnName("vendor_suite_number")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorZip)
                    .HasColumnName("vendor_zip")
                    .HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<VendorContactPerson>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("vendor_contact_person", "productdb");

                entity.HasIndex(e => e.VendorId)
                    .HasName("vendor_id");

                entity.Property(e => e.ContactId)
                    .HasColumnName("contact_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Main)
                    .HasColumnName("main")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Phone1)
                    .IsRequired()
                    .HasColumnName("phone1")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone2)
                    .IsRequired()
                    .HasColumnName("phone2")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<VendorProducts>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.ToTable("vendor_products", "productdb");

                entity.HasIndex(e => e.ProductId)
                    .HasName("product_id");

                entity.HasIndex(e => e.VendorId)
                    .HasName("vendor_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorColorCode)
                    .HasColumnName("vendor_color_code")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorColorName)
                    .HasColumnName("vendor_color_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId)
                    .HasColumnName("vendor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorProductCode)
                    .HasColumnName("vendor_product_code")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VendorProductName)
                    .HasColumnName("vendor_product_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VendorTypes>(entity =>
            {
                entity.HasKey(e => e.VendorTypeId);

                entity.ToTable("vendor_types", "productdb");

                entity.Property(e => e.VendorTypeId)
                    .HasColumnName("vendor_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VendorTypeDescription)
                    .IsRequired()
                    .HasColumnName("vendor_type_description")
                    .IsUnicode(false);
            });
        }
    }
}
