using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebAPITest.Models.Database
{
    public partial class WebEnterpriseIIContext : DbContext
    {
        public WebEnterpriseIIContext()
        {
        }

        public WebEnterpriseIIContext(DbContextOptions<WebEnterpriseIIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Good> Goods { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=119.29.63.234;Initial Catalog=WebEnterpriseII;User ID=stu;Password=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Good>(entity =>
            {
                entity.HasComment("商品表");

                entity.Property(e => e.Id).HasComment("主键");

                entity.Property(e => e.CateId).HasComment("分类Id");

                entity.Property(e => e.Cover)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasComment("封面图");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("时间");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasComment("价格");

                entity.Property(e => e.Stock).HasComment("库存");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName, "UQ__Users__C9F284568CC13987")
                    .IsUnique();

                entity.Property(e => e.UserId).HasComment("用户Id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("注册时间");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasComment("邮件");

                entity.Property(e => e.NickName)
                    .HasMaxLength(255)
                    .HasComment("昵称");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasComment("密码");

                entity.Property(e => e.Salt).HasComment("盐值");

                entity.Property(e => e.UserLevel).HasComment("用户等级（0普通用户 1管理员 2超级管理员）");

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
