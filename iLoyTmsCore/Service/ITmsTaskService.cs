using iLoyTmsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLoyTmsCore.Service
{
    public interface ITmsTaskService
    {
        IEnumerable<TmsTask> GetTmsTasks();
        TmsTask GetTmsTask(int id);
        void InsertTmsTask(TmsTask tmsTask);
        void UpdateTmsTask(TmsTask tmsTask);
        void DeleteTmsTask(int id);

    }
}
