using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Contact_Manager_Application.Interfaces
{
    public interface IFileProcessingService
    {
        Task<(bool Success, string? ErrorMessage)> ProcessFileAsync(IFormFile file);
        Task MergeTempApplicationsAsync();
    }
}