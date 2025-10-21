using APinI.Models;
using Newtonsoft.Json;
using System.Management.Automation;

namespace APinI.Services
{
    public class CicdService : ICicdService
    {
        private readonly List<string> _buildWebsiteScript;
        private readonly List<string> _deployWebsiteScript;
        private string _projectDirectory => "D:\\BackupClones\\NASAboy342.github.io";
        private string _websiteDirectory => "D:\\WebSite\\SpinPort";
        private string _distDirectory => $"{_projectDirectory}\\V2VuePinPort\\V2PinPort\\dist";
        private string _V2PinPortProjectDirectory => $"{_projectDirectory}\\V2VuePinPort\\V2PinPort";

        public CicdService()
        {
            _buildWebsiteScript = new List<string>()
            {
                $"$projectDirectory = \"{_projectDirectory}\"",
                $"$websiteDirectory = \"{_websiteDirectory}\"",
                $"$distDirectory = \"{_distDirectory}\"",
                $"$V2PinPortProjectDirectory = \"{_V2PinPortProjectDirectory}\"",

                "Write-Output \"Going into project directory [\"+$projectDirectory+\"]\"",
                "cd $projectDirectory",

                "write-output \"Start pulling project from project repository\"",
                "git pull origin",
                "write-output \"Pulling done\"",

                "write-output \"Going into V2PinPort project directory [\"+$V2PinPortProjectDirectory+\"]\"",
                "cd $V2PinPortProjectDirectory",

                "write-output \"Start building project\"",
                "npm run build",
                "Write-Output \"Build done\"",
            };
            _deployWebsiteScript = new List<string>()
            {
                "Write-Output \"Going into V2PinPort dist directory [\"+$distDirectory+\"]\"",
                "write-output \"start Copying dists file into website directory [\"+ $websiteDirectory +\"]\"",
                "Get-ChildItem -File -Recurse $distDirectory | ForEach-Object {",
                "    $relativePath = $_.FullName.Substring($distDirectory.Length)",
                "    $destinationPath = Join-Path -Path $websiteDirectory -ChildPath $relativePath",
                "    Copy-Item -Force $_.FullName -Destination $destinationPath",
                "}",
                "Write-Output \"==========================\"",
                "Write-Output \"      Copying done\"",
                "Write-Output \"==========================\"",
            };
        }

        public async Task<UpdateWebsiteResponse> UpdateWebsite()
        {
            var powerShell = PowerShell.Create();
            var resultMessage = new List<string>();

            resultMessage = await BuildWebsite(powerShell, resultMessage);
            resultMessage = await DeployWebsite(powerShell, resultMessage);

            if (resultMessage != null)
            {
                return new UpdateWebsiteResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = "ok",
                    PowershellResponse =  resultMessage
                };
            }
            return new UpdateWebsiteResponse
            {
                ErrorCode = 1,
                ErrorMessage = "null response",
                PowershellResponse = new List<string>()
            };
        }

        private bool isBuildSuccess(List<string> resultMessage)
        {
            return resultMessage.Any(r => r.Contains("mindex.html"));
        }

        private List<string> GetPowerShellOutPutInListOfString(PSDataCollection<PSObject> results)
        {
            var messagesBeforeFormating = results
                .Select(result => result.BaseObject.ToString())?
                .FirstOrDefault()?
                .Split("\n")
                .ToList();
            var messagesAfterFormating = new List<string>();
            messagesBeforeFormating?.ForEach(message => messagesAfterFormating.Add(message.Replace("\r", "")));

            return messagesAfterFormating ?? new List<string>();
        }

        private async Task<List<string>> DeployWebsite(PowerShell powerShell, List<string> resultMessages)
        {
            if (isBuildSuccess(resultMessages))
            {
                powerShell.AddScript(GetScript(_deployWebsiteScript));
                powerShell.AddCommand("Out-String");
                var result = await powerShell.InvokeAsync();
                return resultMessages.Concat(GetPowerShellOutPutInListOfString(result)).ToList();
            }
            else
            {
                resultMessages.Add("=========== Build Was Fail ===========");
                return resultMessages;
            }
        }
        private async Task<List<string>> BuildWebsite(PowerShell powerShell, List<string> resultMessages)
        {
            powerShell.AddScript(GetScript(_buildWebsiteScript));
            powerShell.AddCommand("Out-String");
            var result = await powerShell.InvokeAsync();
            return resultMessages.Concat(GetPowerShellOutPutInListOfString(result)).ToList();
        }

        public string GetScript(List<string> scripts)
        {
            var oneLineScript = String.Empty;

            foreach (var script in scripts)
            {
                oneLineScript += script + "\r\n";
            }
            return oneLineScript;
        }
    }
}