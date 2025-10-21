using APinI.Helppers;
using Microsoft.VisualBasic;

namespace APinI.Schedular
{
    public abstract class JobBase
    {
        private Timer _timer;

        public virtual void Run()
        {
            try
            {
                _timer = new Timer(Execute, null, TimeSpan.Zero, Interval);
            }catch (Exception ex)
            {
                LoggerHelper.Log($"Schedular-Exception: {ex}");
            }
        }

        public abstract TimeSpan Interval { get; set; }

        public abstract void Execute(Object o);
    }
}
