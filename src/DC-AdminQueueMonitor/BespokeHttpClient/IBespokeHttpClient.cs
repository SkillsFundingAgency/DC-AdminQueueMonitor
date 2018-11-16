using System.Threading.Tasks;

namespace DC.Web.Ui.Services.BespokeHttpClient
{
    public interface IBespokeHttpClient
    {
        Task<string> SendDataAsync(string url, object job);

        Task<string> GetDataAsync(string url);
    }
}
