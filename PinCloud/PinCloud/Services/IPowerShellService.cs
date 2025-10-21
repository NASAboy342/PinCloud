using APinI.Models;

namespace APinI.Services
{
    public interface IPowerShellService
    {
        Task<string> RunPowerShellScript(PowerShellRequest request);
        void ValidateScript(string script);
    }
}
