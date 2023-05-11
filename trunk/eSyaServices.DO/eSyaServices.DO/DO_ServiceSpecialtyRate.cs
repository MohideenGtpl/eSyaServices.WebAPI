﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HCP.Services.DO
{
    public class DO_ServiceSpecialtyRate
    {
        public int BusinessKey { get; set; }
        public int ServiceId { get; set; }
        public int SpecialtyId { get; set; }
        public string ServiceTypeDesc { get; set; }
        public string ServiceGroupDesc { get; set; }
        public string ServiceClassDesc { get; set; }
        public string ServiceDesc { get; set; }
        public int RateType { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ServiceRule { get; set; }
        public decimal ServiceRate { get; set; }
        public DateTime? EffectiveTill { get; set; }
        public bool ActiveStatus { get; set; }

        public string FormId { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string TerminalID { get; set; }
    }
}
