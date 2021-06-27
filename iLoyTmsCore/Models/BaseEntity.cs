using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLoyTmsCore.Models
{
    public class BaseEntity
    {
        public Int64 Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
