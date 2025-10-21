using APinI.Helppers;
using System.Net.Sockets;
using System.Net;
using System.Text;
using APinI.Repository;
using APinI.Schedular.Jobs;

namespace APinI.Schedular
{
    public class DeviceSecheduler
    {
        public void Run()
        {
            new LogMoving().Run();
        }
    }
}
