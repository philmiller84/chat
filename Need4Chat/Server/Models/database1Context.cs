using Microsoft.EntityFrameworkCore;

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

        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => new { e.User, e.Timestamp })
                    .HasName("PK__message__977B43D1911DB29B");

                entity.ToTable("message");

                entity.Property(e => e.User).HasColumnName("user");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

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
                    .HasMaxLength(20);

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
