
using APinI.Helppers;
using APinI.Repository;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace APinI.Schedular.Jobs
{
    public class UpdateLocalWebsiteIpAddress : SchedularHostingService
    {
        private readonly PinDataRepository _pinDataRepository;
        
        public UpdateLocalWebsiteIpAddress()
        {
            _pinDataRepository = new PinDataRepository();
        }
        protected override TimeSpan _Interval { get; set; } = TimeSpan.FromMinutes(5);

        public override void ToDo(object state)
        {
            try
            {
                var lastIpAddress = GetLastIpAddress();
                var newIpAddress = GetNewIpAddress();

                var fileToUpdate = File.Open("C:\\Windows\\System32\\drivers\\etc\\hosts", FileMode.Open);
                UpdateFile(fileToUpdate, lastIpAddress, newIpAddress);
                fileToUpdate.Close();
                SetCurrentIpAddress(newIpAddress);
                LoggerHelper.Log($"Update websit ip address DONE. Old ip: [{lastIpAddress}] New ip: [{newIpAddress}]");
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Update websit ip address ERROR. Exception [{ex}]");
            }
        }
        private void SetCurrentIpAddress(string newIpAddress)
        {
            _pinDataRepository.SetCurrentIpAddress(newIpAddress);
        }

        private void UpdateFile(FileStream fileToUpdate, string lastIpAddress, string newIpAddress)
        {
            var streamReader = new StreamReader(fileToUpdate);
            var textInFile = streamReader.ReadToEnd();
            LoggerHelper.Log($"host file read: [{textInFile}]");
            textInFile = textInFile.Replace(lastIpAddress, newIpAddress);
            fileToUpdate.SetLength(0);
            fileToUpdate.Write(Encoding.UTF8.GetBytes(textInFile));
            fileToUpdate.Flush();
        }
        private string GetNewIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private string GetLastIpAddress()
        {
            return _pinDataRepository.GetLastIpAddress().LastIpAddress;
        }
    }
}
