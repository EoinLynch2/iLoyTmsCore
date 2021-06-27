using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLoyTmsCore.Models
{
    public class TmsTaskMap
    {
        public TmsTaskMap(EntityTypeBuilder<TmsTask> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.Property(t => t.TaskName).IsRequired();
            entityBuilder.Property(t => t.StartDate);
            entityBuilder.Property(t => t.FinishDate);
            entityBuilder.Property(t => t.State).IsRequired();
            entityBuilder.HasOne(t => t.ParentTmsTask).WithMany(t => t.Subtasks).HasForeignKey(t => t.ParentTmsTaskId);
                

        }

    }
}
