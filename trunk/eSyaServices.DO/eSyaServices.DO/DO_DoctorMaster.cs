﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HCP.Services.DO
{
    public class DO_DoctorMaster
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorShortName { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public string DoctorRegnNo { get; set; }
        public int Isdcode { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public int DoctorClass { get; set; }
        public int DoctorCategory { get; set; }
        public bool AllowConsultation { get; set; }
        public bool IsRevenueShareApplicable { get; set; }
        public bool AllowSms { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
