using RL.Data.DataModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RL.Data.DataModels
{
    public class PlanProcedureUser : IChangeTrackable
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual PlanProcedure PlanProcedure { get; set; }
        public virtual User User { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
