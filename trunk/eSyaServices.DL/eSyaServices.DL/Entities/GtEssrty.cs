using System;
using System.Collections.Generic;

namespace HCP.Services.DL.Entities
{
    public partial class GtEssrty
    {
        public GtEssrty()
        {
            GtEssrgr = new HashSet<GtEssrgr>();
        }

        public int ServiceTypeId { get; set; }
        public string ServiceTypeDesc { get; set; }
        public int PrintSequence { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedTerminal { get; set; }

        public virtual ICollection<GtEssrgr> GtEssrgr { get; set; }
    }
}
