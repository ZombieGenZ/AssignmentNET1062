using Assignment.Dtos.Admin;

namespace Assignment.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync(int rangeDays);
    }
}
