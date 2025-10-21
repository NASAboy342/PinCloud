using APinI.Helppers;

namespace APinI.Schedular
{
    public abstract class SchedularHostingService : IHostedService, IAsyncDisposable
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private int _executionCount = 0;
        private Timer? _timer;
        protected abstract TimeSpan _Interval { get; set; } 
        public Task StartAsync(CancellationToken stoppingToken)
        {
            LoggerHelper.Log($"{nameof(SchedularHostingService)} is running.");
            _timer = new Timer(ToDo, null, TimeSpan.Zero, _Interval);
            return _completedTask;
        }

        public abstract void ToDo(object? state);

        public Task StopAsync(CancellationToken stoppingToken)
        {
            LoggerHelper.Log($"{nameof(SchedularHostingService)} is stopping.");
            return _completedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Delay(100);
        }
    }
}
