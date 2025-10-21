using APinI.Models;

namespace APinI.Services
{
    public interface ICicdService
    {
        Task<UpdateWebsiteResponse> UpdateWebsite();
    }
}
