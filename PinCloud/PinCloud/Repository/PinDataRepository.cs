using APinI.Models;
using APinI.Helppers;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace APinI.Repository
{
    public class PinDataRepository
    {
        private readonly IDbConnection GetConnection;
        public PinDataRepository()
        {
            GetConnection = SqlConnection();
            GetConnection.Open();
        }
        private SqlConnection SqlConnection()
        {
            var cofig = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"];
            var connectionstring = cofig?.ConnectionString ?? "Data Source=.;Initial Catalog=PinData;User Id=sa;Password=1234qwer";
            return new SqlConnection(connectionstring);
        }
        public IEnumerable<T> GetData<T>(string spName)
        {
            var data = GetConnection.Query<T>(spName, null,null,true,null,CommandType.StoredProcedure);
            return data;
        } 
        public IEnumerable<T> GetData<T>(string spName,object param)
        {
            var data = GetConnection.Query<T>(spName, param, null, true, null, CommandType.StoredProcedure);
            return data;
        }
        internal List<GetAllTaskResponse> GetAllTask()
        {
            return GetData<GetAllTaskResponse>("GetAllTask").ToList();
        }
        internal List<GetAllTaskResponse> GetAllTask(GetAllTaskReques req)
        {
            return GetData<GetAllTaskResponse>("GetAllTaskByStatus",new
            {
                req.Status
            }).ToList();
        }

        internal BaseResponse AddTask(BaseTask req)
        {
            return GetData<BaseResponse>("AddTask", new
            {
                req.Name,
                req.Description,
                req.Status
            }).FirstOrDefault();
        }

        internal WebsiteIpAddress GetLastIpAddress()
        {
            return GetData<WebsiteIpAddress>("[dbo].[GetPreviousIp]").FirstOrDefault();
        }

        internal void SetCurrentIpAddress(string newIpAddress)
        {
            GetData<BaseResponse>("[dbo].[SetCurrentIp]",new
            {
                newIpAddress
            }).FirstOrDefault();
        }

        internal void SaveCandles(List<IQOptionCandle> candles)
        {
            GetData<BaseResponse>("[dbo].[AddIQOptionCandle]", new
            {
                Candle = candles.ToDataTable()
            }).FirstOrDefault();
        }
    }
}
