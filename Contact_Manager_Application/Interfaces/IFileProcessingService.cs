using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Contact_Manager_Application.Interfaces
{
    public interface IFileProcessingService
    {
        Task ProcessFileAsync(IFormFile file);
        Task MergeTempApplicationsAsync();
    }
}