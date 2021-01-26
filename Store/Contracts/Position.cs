using System;
using System.Collections.Generic;
using System.Text;

namespace Flexoft.ForexManager.Store.Contracts
{
    public class Position
    {
        public int Id { get; set; }
        
        public string FromCurrency { get; set; }
        
        public string ToCurrency { get; set; }

        public double OpenAmount { get; set; }

        public DateTime OpenStamp { get; set; }

        public double OpenRate { get; set; }

        public DateTime? CloseStamp { get; set; }

        public double? CloseRate { get; set; }

        public double? CloseAmount { get; set; }

        public double? Diff { get; set; }

        public double? Fee { get; set; }
    }
}
