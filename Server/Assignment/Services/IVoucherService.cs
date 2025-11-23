using Assignment.Dtos.Vouchers;

namespace Assignment.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetPublicVouchersAsync();
        Task<List<VoucherAdminDto>> GetAllAsync();
        Task<VoucherAdminDto?> GetByIdAsync(Guid id);
        Task<VoucherAdminDto> CreateAsync(VoucherUpsertDto model);
        Task<VoucherAdminDto?> UpdateAsync(Guid id, VoucherUpsertDto model);
        Task<bool> DeleteAsync(Guid id);
        Task<ValidateVoucherResponse> ValidateVoucherAsync(ValidateVoucherRequest request);
    }
}
