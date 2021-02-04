using System.Threading.Tasks;

namespace Redcap.Services
{
    public interface IApiService
    {
        Task<T> ExportRecordAsync<T>();
    }
}