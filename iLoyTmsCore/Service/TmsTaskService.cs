using iLoyTmsCore.Models;
using iLoyTmsCore.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLoyTmsCore.Service
{
    public class TmsTaskService : ITmsTaskService
    {
        private IRepository<TmsTask> tmsTaskRepository;
        

        public TmsTaskService(IRepository<TmsTask> tmsTaskRepository)
        {
            this.tmsTaskRepository = tmsTaskRepository;
        
        }

        public IEnumerable<TmsTask> GetTmsTasks()
        {
            return tmsTaskRepository.GetAll();
        }

        public TmsTask GetTmsTask(int id)
        {
            return tmsTaskRepository.Get(id);
        }

        public void InsertTmsTask(TmsTask tmsTask)
        {
            tmsTaskRepository.Insert(tmsTask);
        }

        public void UpdateTmsTask(TmsTask tmsTask)
        {
            tmsTaskRepository.Update(tmsTask);
        }

        public void DeleteTmsTask(int id)
        {
    
            TmsTask tmsTask = GetTmsTask(id);
            tmsTaskRepository.Remove(tmsTask);
            tmsTaskRepository.SaveChanges();
        }
    }
}
