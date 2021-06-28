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

        public void DeleteTmsTask(TmsTask tmsTask)
        {
            tmsTaskRepository.Remove(tmsTask);
            tmsTaskRepository.SaveChanges();
        }

        public int GetNoOfSubtasksForTmsTask(TmsTask tmsTask)
        {
            int numSubtasks = tmsTask.Subtasks.Count;
            return numSubtasks;
        }


        public void UpdateParentTaskStatus(int ParentTaskId)
        {
            string parentTaskState = "Planned";
            TmsTask tmsTask = tmsTaskRepository.Get(ParentTaskId);

            int totalSubtasks = GetNoOfSubtasksForTmsTask(tmsTask);

            int totalCompletedSubtasks = tmsTask.Subtasks.Where(t => t.ParentTmsTaskId == ParentTaskId && t.State == "Completed").Count();


            if (totalCompletedSubtasks == totalSubtasks)
            {
                parentTaskState = "Completed";
            }
            else
            {
                int totalInProgressSubtasks = tmsTask.Subtasks
                    .Where(t => t.ParentTmsTaskId == ParentTaskId && t.State == "InProgress").Count();
                if (totalInProgressSubtasks > 0)
                    parentTaskState = "InProgress";
            }

            tmsTask.State = parentTaskState;
            tmsTaskRepository.Update(tmsTask);
        }
    }
}
