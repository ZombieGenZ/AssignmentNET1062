using Assignment.Dtos.Vouchers;

namespace Assignment.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetPublicVouchersAsync();
        Task<ValidateVoucherResponse> ValidateVoucherAsync(ValidateVoucherRequest request);
    }
}
