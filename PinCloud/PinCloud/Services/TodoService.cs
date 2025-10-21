using APinI.Models;
using APinI.Repository;

namespace APinI.Services
{
    public class TodoService
    {
        private readonly PinDataRepository _pinDataRepository = new PinDataRepository();

        public List<GetAllTaskResponse> GetAllTask()
        {
            return _pinDataRepository.GetAllTask();
        }
        public List<GetAllTaskResponse> GetAllTask(GetAllTaskReques req)
        {
            return _pinDataRepository.GetAllTask(req);
        }

        public BaseResponse AddTask(BaseTask req)
        {
            return _pinDataRepository.AddTask(req);
        }
    }
}
