namespace APinI.Services
{
    public interface IHttpClientService
    {
        Task<TResponse> Get<TResponse>(string url) where TResponse : class;

        Task<TResponse> Post<TResponse, TRequest>(string url, TRequest req) where TResponse : class;

        Task<TResponse> Post<TResponse>(string url) where TResponse : class;
        Task<string> Post(string url);

        Task<TResponse> Form<TResponse>(string url, Dictionary<string, string> req)
            where TResponse : class;

        Task<bool> CheckUrlValidity(string url);
    }
}