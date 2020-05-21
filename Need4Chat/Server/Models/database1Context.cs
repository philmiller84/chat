using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Need4Chat.Server.Models
{
    public partial class database1Context : DbContext
    {
        public database1Context()
        {
        }

        public database1Context(DbContextOptions<database1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Trade> Trade { get; set; }
        public virtual DbSet<TradeOffset> TradeOffset { get; set; }
        public virtual DbSet<TradeRequirement> TradeRequirement { get; set; }
        public virtual DbSet<TradeUser> TradeUser { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(200);

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.ToTable("trade");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<TradeOffset>(entity =>
            {
                entity.HasKey(e => new { e.TradeRequirementId, e.UserId })
                    .HasName("PK__trade_of__C532413FE66B3A8B");

                entity.ToTable("trade_offset");

                entity.Property(e => e.TradeRequirementId).HasColumnName("trade_requirement_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Offset).HasColumnName("offset");
            });

            modelBuilder.Entity<TradeRequirement>(entity =>
            {
                entity.HasKey(e => new { e.TradeId, e.ItemId })
                    .HasName("PK__trade_re__6FDF7B0A880B38D3");

                entity.ToTable("trade_requirement");

                entity.Property(e => e.TradeId).HasColumnName("trade_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Offset).HasColumnName("offset");
            });

            modelBuilder.Entity<TradeUser>(entity =>
            {
                entity.HasKey(e => new { e.TradeId, e.UserId })
                    .HasName("PK__trade_us__4164B887D393579C");

                entity.ToTable("trade_user");

                entity.Property(e => e.TradeId).HasColumnName("trade_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(22);

                entity.Property(e => e.PasswordHint)
                    .IsRequired()
                    .HasColumnName("password_hint")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('Do you remember your password?')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
