using APinI.Helppers;
using APinI.Services;
using Newtonsoft.Json;

namespace APinI.Schedular.Jobs
{
    public class SchedulerHealthCheck : SchedularHostingService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _hostBaseUrl;
        private TimeSpan _schedulerLifeSpend = TimeSpan.FromMinutes(0);
        public SchedulerHealthCheck(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _hostBaseUrl = configuration.GetValue<string>("HostBaseUrl");
        }
        protected override TimeSpan _Interval { get; set; } = TimeSpan.FromMinutes(5);

        public override void ToDo(object? state)
        {
            try
            {
                var url = $"{_hostBaseUrl}api/CicdController/trigger-api-usage";
                var response = _httpClientService.Post(url);
                LoggerHelper.Log($"Call self invoke url: {url} respose: {JsonConvert.SerializeObject(response.Result)}");
                _schedulerLifeSpend += _Interval;
                LoggerHelper.Log($"[{nameof(SchedulerHealthCheck)}] scheduler has beed running for {Convert.ToString(_schedulerLifeSpend)}");
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"[{nameof(SchedulerHealthCheck)}] fail at {Convert.ToString(_schedulerLifeSpend)} due to: {ex}");
            }
        }
    }
}