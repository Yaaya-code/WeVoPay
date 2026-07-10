using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface ITransferService
    {
        Task<List<TransferRequest>> GetAllTransfersAsync();

        Task<List<TransferRequest>> GetUserTransfersAsync(
            int userId,
            string? search,
            TransferStatus? status,
            int page,
            int pageSize);

        Task<int> GetUserTransfersCountAsync(
            int userId,
            string? search,
            TransferStatus? status);

        Task<TransferRequest?> GetUserTransferByIdAsync(int transferId, int userId);

        Task<TransferRequest> CreateTransferAsync(int userId, CreateTransferDto dto);

        Task<bool> VerifyTransferAsync(int transferId, int adminId);

        Task<bool> CompleteTransferAsync(int transferId, int adminId);

        Task<bool> RejectTransferAsync(int transferId, int adminId);

        Task<TransferRequest?> GetTransferByIdAsync(int id);

        Task<(List<TransferRequest> Transfers, int TotalCount)> GetPendingTransfersAsync(
            string? search,
            int page,
            int pageSize);

        Task<(List<TransferRequest> Transfers, int TotalCount)> GetVerifiedTransfersAsync(
            string? search,
            int page,
            int pageSize);

        Task<(List<TransferRequest> Transfers, int TotalCount)> GetCompletedTransfersAsync(
            string? search,
            int page,
            int pageSize);

        Task<(List<TransferRequest> Transfers, int TotalCount)> GetRejectedTransfersAsync(
            string? search,
            int page,
            int pageSize);

        Task<int> CancelExpiredPendingTransfersAsync();
    }
}
