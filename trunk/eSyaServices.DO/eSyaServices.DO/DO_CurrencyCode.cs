using System;
using System.Collections.Generic;
using System.Text;

namespace HCP.Services.DO
{
    public class DO_CurrencyCode
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public decimal DecimalPlaces { get; set; }
        public bool ShowInMillions { get; set; }
        public bool SymbolSuffixToAmount { get; set; }
        public string DecimalPortionWord { get; set; }
        public bool UsageStatus { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
