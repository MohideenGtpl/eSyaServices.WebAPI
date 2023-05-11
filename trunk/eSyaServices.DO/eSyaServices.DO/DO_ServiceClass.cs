﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HCP.Services.DO
{
    public class DO_ServiceClass
    {
        public int ServiceClassId { get; set; }
        public int ServiceGroupId { get; set; }
        public string ServiceClassDesc { get; set; }
        public bool IsBaseRateApplicable { get; set; }
        public int ParentId { get; set; }
        public int PrintSequence { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string TerminalID { get; set; }

        public List<DO_eSyaParameter> l_ClassParameter { get; set; }

    }
}
