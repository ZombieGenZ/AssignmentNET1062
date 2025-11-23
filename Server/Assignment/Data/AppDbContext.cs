using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Data
{
    public class AppDbContext
        : IdentityDbContext<AppUser, AppRole, Guid,
                            IdentityUserClaim<Guid>, IdentityUserRole<Guid>,
                            IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
                            IdentityUserToken<Guid>>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ComboItem many-to-many
            builder.Entity<ComboItem>()
                .HasKey(ci => new { ci.ComboId, ci.ProductId });

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Combo)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.ComboId);

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.ComboItems)
                .HasForeignKey(ci => ci.ProductId);

            // CartItem
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            // RefreshToken unique
            builder.Entity<RefreshToken>()
                .HasIndex(x => x.Token)
                .IsUnique();
        }
    }

}
