using Assignment.Data;
using Assignment.Dtos.Admin;
using Assignment.Enums;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _db;

        public AdminDashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync(int rangeDays)
        {
            var today = DateTime.UtcNow.Date;
            var safeRange = Math.Clamp(rangeDays, 1, 90);
            var startDate = today.AddDays(1 - safeRange);

            var ordersInRange = _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedAt.Date >= startDate && o.CreatedAt.Date <= today);

            var revenueByDate = await ordersInRange
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice),
                    Orders = g.Count(),
                    OnlineOrders = g.Count(o => o.PaymentMethod == PaymentMethod.PayNow)
                })
                .ToListAsync();

            var revenueMap = revenueByDate.ToDictionary(x => DateOnly.FromDateTime(x.Date));
            var revenueSeries = new List<RevenuePointDto>();
            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                var key = DateOnly.FromDateTime(date);
                if (revenueMap.TryGetValue(key, out var value))
                {
                    revenueSeries.Add(new RevenuePointDto
                    {
                        Date = key,
                        Revenue = value.Revenue,
                        Orders = value.Orders,
                        OnlineOrders = value.OnlineOrders
                    });
                }
                else
                {
                    revenueSeries.Add(new RevenuePointDto
                    {
                        Date = key,
                        Revenue = 0,
                        Orders = 0,
                        OnlineOrders = 0
                    });
                }
            }

            var totalRevenue = revenueSeries.Sum(s => s.Revenue);
            var totalOrders = revenueSeries.Sum(s => s.Orders);
            var onlinePaymentCount = revenueSeries.Sum(s => s.OnlineOrders);
            var avgOrderValue = totalOrders > 0 ? Math.Round(totalRevenue / totalOrders, 0) : 0;
            var onlinePaymentRate = totalOrders > 0
                ? (int)Math.Round((double)onlinePaymentCount / totalOrders * 100)
                : 0;

            var previousStart = startDate.AddDays(-safeRange);
            var previousEnd = startDate.AddDays(-1);
            var previousRevenue = await _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedAt.Date >= previousStart && o.CreatedAt.Date <= previousEnd)
                .SumAsync(o => o.TotalPrice);
            var revenueGrowth = previousRevenue > 0
                ? (int)Math.Round((double)((totalRevenue - previousRevenue) / previousRevenue) * 100)
                : 0;

            var completedOrders = await ordersInRange.CountAsync(o => o.Status == OrderStatus.Completed);
            var fulfillmentRate = totalOrders > 0
                ? (int)Math.Round((double)completedOrders / totalOrders * 100)
                : 0;

            var recentOrders = await _db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(8)
                .Select(o => new DashboardOrderDto
                {
                    Id = o.Id,
                    Code = $"#FF-{o.CreatedAt:yyMMdd}-{o.Id.ToString()[..6].ToUpper()}",
                    CustomerName = string.IsNullOrWhiteSpace(o.CustomerName) ? "Khách vãng lai" : o.CustomerName,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            var bestSellers = await _db.OrderItems
                .AsNoTracking()
                .Include(oi => oi.Order)
                .Include(oi => oi.Product).ThenInclude(p => p.Category)
                .Include(oi => oi.Combo)
                .Where(oi => oi.Order.CreatedAt.Date >= startDate && oi.Order.CreatedAt.Date <= today)
                .GroupBy(oi => new
                {
                    oi.ItemType,
                    oi.ProductId,
                    oi.ComboId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ComboName = oi.Combo != null ? oi.Combo.Name : null,
                    Category = oi.Product != null ? oi.Product.Category.Name : "Combo"
                })
                .Select(g => new BestSellerDto
                {
                    Name = g.Key.ProductName ?? g.Key.ComboName ?? "Sản phẩm",
                    Category = g.Key.Category ?? "Khác",
                    Sold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Quantity * i.UnitPrice)
                })
                .OrderByDescending(x => x.Sold)
                .ThenByDescending(x => x.Revenue)
                .Take(5)
                .ToListAsync();

            var summary = new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                AvgOrderValue = avgOrderValue,
                OnlinePaymentRate = onlinePaymentRate,
                RevenueGrowth = revenueGrowth,
                FulfillmentRate = fulfillmentRate
            };

            return new AdminDashboardDto
            {
                Summary = summary,
                RevenueSeries = revenueSeries,
                RecentOrders = recentOrders,
                BestSellers = bestSellers
            };
        }
    }
}
