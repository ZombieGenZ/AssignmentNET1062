using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Tự động migrate database nếu chưa có
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new AppRole { Name = "Admin", Description = "Quản trị viên hệ thống" });
            if (!await roleManager.RoleExistsAsync("Staff"))
                await roleManager.CreateAsync(new AppRole { Name = "Staff", Description = "Nhân viên" });
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new AppRole { Name = "User", Description = "Khách hàng" });

            // 2. Seed Users
            if (await userManager.FindByEmailAsync("admin@fastfood.com") == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin@fastfood.com",
                    Email = "admin@fastfood.com",
                    FullName = "Administrator",
                    PhoneNumber = "0909000111",
                    Address = "Hồ Chí Minh",
                    Gender = "male",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("user@fastfood.com") == null)
            {
                var user = new AppUser
                {
                    UserName = "user@fastfood.com",
                    Email = "user@fastfood.com",
                    FullName = "Khách Hàng Mẫu",
                    PhoneNumber = "0909000222",
                    Address = "123 Nguyễn Huệ, Quận 1",
                    Gender = "female",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }

            // 3. Seed Categories (Nếu chưa có)
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Burger", IsActive = true },
                    new Category { Id = Guid.NewGuid(), Name = "Gà Rán", IsActive = true },
                    new Category { Id = Guid.NewGuid(), Name = "Pizza", IsActive = true },
                    new Category { Id = Guid.NewGuid(), Name = "Món Ăn Kèm", IsActive = true },
                    new Category { Id = Guid.NewGuid(), Name = "Đồ Uống", IsActive = true }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // 4. Seed Products
            if (!context.Products.Any())
            {
                var cats = await context.Categories.ToListAsync();
                var catBurger = cats.First(c => c.Name == "Burger").Id;
                var catChicken = cats.First(c => c.Name == "Gà Rán").Id;
                var catPizza = cats.First(c => c.Name == "Pizza").Id;
                var catSide = cats.First(c => c.Name == "Món Ăn Kèm").Id;
                var catDrink = cats.First(c => c.Name == "Đồ Uống").Id;

                var products = new List<Product>
                {
                    // Burger
                    new Product { Id = Guid.NewGuid(), CategoryId = catBurger, Name = "Classic Beef Burger", Price = 69000, Description = "Bò nướng lửa hồng, phô mai cheddar, rau diếp.", ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catBurger, Name = "Spicy Chicken Burger", Price = 59000, Description = "Gà giòn cay nồng, sốt mayo đặc biệt.", ImageUrl = "https://images.unsplash.com/photo-1615297928064-24977384d0f9?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catBurger, Name = "Double Cheese Burger", Price = 89000, Description = "Hai lớp bò, hai lớp phô mai tan chảy.", ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    
                    // Gà Rán
                    new Product { Id = Guid.NewGuid(), CategoryId = catChicken, Name = "Gà Rán Giòn (1 miếng)", Price = 35000, Description = "Da giòn rụm, thịt mềm ngọt.", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catChicken, Name = "Cánh Gà Sốt BBQ (3 miếng)", Price = 55000, Description = "Sốt BBQ đậm đà hương vị khói.", ImageUrl = "https://images.unsplash.com/photo-1567620832903-9fc6debc209f?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catChicken, Name = "Gà Viên Popcorn", Price = 45000, Description = "Viên gà nhỏ vừa miệng, thích hợp ăn vặt.", ImageUrl = "https://images.unsplash.com/photo-1569691899455-88464f6d3ab1?auto=format&fit=crop&w=800&q=80", IsActive = true },

                    // Pizza
                    new Product { Id = Guid.NewGuid(), CategoryId = catPizza, Name = "Pizza Pepperoni", Price = 129000, Description = "Xúc xích Pepperoni và phô mai Mozzarella.", ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catPizza, Name = "Pizza Hải Sản", Price = 159000, Description = "Tôm, mực, nghêu và sốt pesto.", ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catPizza, Name = "Pizza Phô Mai", Price = 119000, Description = "4 loại phô mai cao cấp.", ImageUrl = "https://images.unsplash.com/photo-1574071318500-8c675212cc6f?auto=format&fit=crop&w=800&q=80", IsActive = true },

                    // Món ăn kèm
                    new Product { Id = Guid.NewGuid(), CategoryId = catSide, Name = "Khoai Tây Chiên", Price = 25000, Description = "Khoai tây chiên vàng giòn.", ImageUrl = "https://images.unsplash.com/photo-1630384060421-cb20d0e0649d?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catSide, Name = "Khoai Tây Lắc Phô Mai", Price = 32000, Description = "Thêm bột phô mai thơm béo.", ImageUrl = "https://images.unsplash.com/photo-1573080496982-4971a575b055?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catSide, Name = "Salad Cầu Vồng", Price = 39000, Description = "Rau củ tươi mát kèm sốt mè rang.", ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=800&q=80", IsActive = true },

                    // Đồ uống
                    new Product { Id = Guid.NewGuid(), CategoryId = catDrink, Name = "Coca Cola Tươi", Price = 15000, Description = "Ly lớn mát lạnh.", ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catDrink, Name = "Trà Đào Cam Sả", Price = 35000, Description = "Thanh mát giải nhiệt.", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=800&q=80", IsActive = true },
                    new Product { Id = Guid.NewGuid(), CategoryId = catDrink, Name = "Trà Sữa Trân Châu", Price = 42000, Description = "Trân châu đen dai ngon.", ImageUrl = "https://images.unsplash.com/photo-1558855410-3112e253d704?auto=format&fit=crop&w=800&q=80", IsActive = true },
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // 5. Seed Combos
            if (!context.Combos.Any())
            {
                var products = await context.Products.ToListAsync();
                var burger = products.First(p => p.Name.Contains("Classic Beef"));
                var chicken = products.First(p => p.Name.Contains("Gà Rán Giòn"));
                var fries = products.First(p => p.Name.Contains("Khoai Tây Chiên"));
                var coke = products.First(p => p.Name.Contains("Coca Cola"));
                var pizza = products.First(p => p.Name.Contains("Pizza Pepperoni"));

                var combo1 = new Combo
                {
                    Id = Guid.NewGuid(),
                    Name = "Combo Burger Solo",
                    Description = "1 Burger Bò + 1 Khoai + 1 Nước. Ăn nhanh gọn lẹ.",
                    ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?auto=format&fit=crop&w=800&q=80",
                    DiscountPercent = 15,
                    IsActive = true
                };

                var combo2 = new Combo
                {
                    Id = Guid.NewGuid(),
                    Name = "Combo Gà Rán Vui Vẻ",
                    Description = "2 Miếng Gà + 1 Khoai + 1 Nước. Giòn tan đậm vị.",
                    ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?auto=format&fit=crop&w=800&q=80",
                    DiscountPercent = 10,
                    IsActive = true
                };

                var combo3 = new Combo
                {
                    Id = Guid.NewGuid(),
                    Name = "Combo Couple Pizza",
                    Description = "1 Pizza Pepperoni + 2 Nước + 1 Salad. Lãng mạn 2 người.",
                    ImageUrl = "https://images.unsplash.com/photo-1574126154517-d1e0d89e7344?auto=format&fit=crop&w=800&q=80",
                    DiscountPercent = 20,
                    IsActive = true
                };

                context.Combos.AddRange(combo1, combo2, combo3);
                await context.SaveChangesAsync();

                // Link items
                var comboItems = new List<ComboItem>
                {
                    new ComboItem { ComboId = combo1.Id, ProductId = burger.Id, Quantity = 1 },
                    new ComboItem { ComboId = combo1.Id, ProductId = fries.Id, Quantity = 1 },
                    new ComboItem { ComboId = combo1.Id, ProductId = coke.Id, Quantity = 1 },

                    new ComboItem { ComboId = combo2.Id, ProductId = chicken.Id, Quantity = 2 },
                    new ComboItem { ComboId = combo2.Id, ProductId = fries.Id, Quantity = 1 },
                    new ComboItem { ComboId = combo2.Id, ProductId = coke.Id, Quantity = 1 },

                    new ComboItem { ComboId = combo3.Id, ProductId = pizza.Id, Quantity = 1 },
                    new ComboItem { ComboId = combo3.Id, ProductId = coke.Id, Quantity = 2 }
                };
                context.ComboItems.AddRange(comboItems);
                await context.SaveChangesAsync();
            }

            // 6. Seed Vouchers
            if (!context.Vouchers.Any())
            {
                var vouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "WELCOME50", Description = "Giảm 50% cho đơn đầu tiên",
                        DiscountPercent = 50, MinOrderValue = 0, MaxUsage = 1000, IsPublic = true,
                        StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddMonths(3), IsActive = true
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "FREESHIP", Description = "Miễn phí vận chuyển (giảm tối đa 30k)",
                        DiscountAmount = 30000, MinOrderValue = 150000, MaxUsage = 500, IsPublic = true,
                        StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddMonths(1), IsActive = true
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "TET2025", Description = "Lì xì 20% nhân dịp Tết",
                        DiscountPercent = 20, MinOrderValue = 200000, MaxUsage = 200, IsPublic = true,
                        StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), IsActive = true
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "SUMMER10", Description = "Mát lạnh mùa hè giảm 10%",
                        DiscountPercent = 10, MinOrderValue = 50000, MaxUsage = 100, IsPublic = true,
                        StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddMonths(2), IsActive = true
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "VIPMEMBER", Description = "Voucher riêng cho khách VIP",
                        DiscountPercent = 25, MinOrderValue = 500000, MaxUsage = 50, IsPublic = false, // Private
                        StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddYears(1), IsActive = true
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid(), Code = "EXPIRED99", Description = "Voucher đã hết hạn test",
                        DiscountPercent = 99, MinOrderValue = 0, MaxUsage = 10, IsPublic = true,
                        StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-1), IsActive = false
                    }
                };
                context.Vouchers.AddRange(vouchers);
                await context.SaveChangesAsync();
            }
        }
    }
}
