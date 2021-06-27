using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace iLoyTmsCore.Models
{
    public class TmsTask : BaseEntity
    {
        //public int TmsTaskId { get; set; }
        /*[StringLength(200)]
        [Index(IsUnique = true)]*/
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string State { get; set; }


        public int? ParentTmsTaskId { get; set; }
        public virtual TmsTask ParentTmsTask { get; set; }
        public virtual HashSet<TmsTask> Subtasks { get; set; }
    }
}