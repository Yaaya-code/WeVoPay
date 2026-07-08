using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface ITransferService
    {
        Task<List<TransferRequest>> GetAllTransfersAsync();

        Task<List<TransferRequest>> GetUserTransfersAsync(int userId);

        Task<TransferRequest?> GetUserTransferByIdAsync(int transferId, int userId);

        Task<TransferRequest> CreateTransferAsync(int userId, CreateTransferDto dto);

        Task<bool> VerifyTransferAsync(int transferId, int adminId);

        Task<bool> CompleteTransferAsync(int transferId, int adminId);

        Task<bool> RejectTransferAsync(int transferId, int adminId);

        Task<List<TransferRequest>> GetPendingTransfersAsync();

        Task<List<TransferRequest>> GetVerifiedTransfersAsync();

    }
}