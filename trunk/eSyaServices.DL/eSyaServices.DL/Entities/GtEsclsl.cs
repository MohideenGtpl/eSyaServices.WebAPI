﻿using System;
using System.Collections.Generic;

namespace HCP.Services.DL.Entities
{
    public partial class GtEsclsl
    {
        public int BusinessKey { get; set; }
        public int ClinicId { get; set; }
        public int ConsultationId { get; set; }
        public int ServiceId { get; set; }
        public int VisitRule { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedTerminal { get; set; }
    }
}
