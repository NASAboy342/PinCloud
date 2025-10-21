using APinI.Models;
using System.Management.Automation;
using System.Text;

namespace APinI.Services
{
    public class PowerShellService : IPowerShellService
    {
        public async Task<string> RunPowerShellScript(PowerShellRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        // Add the script to be executed
                        powerShell.AddScript(request.Script);

                        // Execute the script
                        var results = powerShell.Invoke();
                        
                        var output = new StringBuilder();
                        
                        // Process the results
                        foreach (var result in results)
                        {
                            if (result != null)
                            {
                                output.AppendLine(result.ToString());
                            }
                        }

                        // Check for errors
                        if (powerShell.HadErrors)
                        {
                            var errorOutput = new StringBuilder();
                            errorOutput.AppendLine("Errors occurred during script execution:");
                            
                            foreach (var error in powerShell.Streams.Error)
                            {
                                errorOutput.AppendLine($"Error: {error}");
                            }
                            
                            // Append errors to the main output
                            output.AppendLine(errorOutput.ToString());
                        }

                        return output.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return $"PowerShell execution failed: {ex.Message}\n{ex.StackTrace}";
                }
            });
        }

        public void ValidateScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentException("Script cannot be null or empty.");
            }
        }
    }
}
