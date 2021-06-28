using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using iLoyTmsCore.Models.Validation;

namespace iLoyTmsCore.Models
{
    public class TmsTask : BaseEntity
    {
        //public int TmsTaskId { get; set; }
        /*[StringLength(200)]
        [Index(IsUnique = true)]*/
        [Required(ErrorMessage = "Task name cannot be null or empty")]
        public string TaskName { get; set; }
        public string Description { get; set; }
        [DateNotInFuture(ErrorMessage = "Date must not be in the future")]
        public DateTime StartDate { get; set; }
        [DateNotInFuture(ErrorMessage = "Date must not be in the future")]
        public DateTime FinishDate { get; set; }
        [StringRange(AllowableValues = new[] { "InProgress", "Completed", "Planned" }, ErrorMessage = "State must either be Inprogress, Completed or Planned")]
        public string State { get; set; }


        public int? ParentTmsTaskId { get; set; }
        public virtual TmsTask ParentTmsTask { get; set; }
        public virtual HashSet<TmsTask> Subtasks { get; set; }

        
    }
}