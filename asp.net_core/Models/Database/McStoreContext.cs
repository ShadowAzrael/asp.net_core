using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MVC_Demo.Models.Database
{
    public partial class McStoreContext : DbContext
    {
        public McStoreContext()
        {
        }

        public McStoreContext(DbContextOptions<McStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Good> Goods { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=119.29.63.234;Initial Catalog=McStore;User ID=sa;password=aa520.0.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__Cars__FBDF78E9C1BB67E9");

                entity.Property(e => e.RecordId).HasComment("记录Id,主键,自增");

                entity.Property(e => e.Count).HasComment("商品数量");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.GoodId).HasComment("商品Id");

                entity.Property(e => e.UserId).HasComment("用户Id");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasComment("主键");

                entity.Property(e => e.CateName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasComment("分类名称");
            });

            modelBuilder.Entity<Good>(entity =>
            {
                entity.HasComment("商品表");

                entity.Property(e => e.Id).HasComment("主键");

                entity.Property(e => e.Cover)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("时间");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Stock).HasComment("库存");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasComment("主键");

                entity.Property(e => e.CraeteTime)
                    .HasColumnType("datetime")
                    .HasComment("注册时间");

                entity.Property(e => e.Desc)
                    .HasMaxLength(1024)
                    .HasComment("个人简介");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasComment("密码");

                entity.Property(e => e.Photo)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasComment("头像");

                entity.Property(e => e.Str)
                    .HasMaxLength(255)
                    .HasComment("盐值");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasComment("用户名");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
