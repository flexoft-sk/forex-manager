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

        public float OpenAmount { get; set; }

        public DateTime OpenStamp { get; set; }

        public float OpenRate { get; set; }

        public DateTime? CloseStamp { get; set; }

        public float CloseRate { get; set; }

        public float CloseAmount { get; set; }

        public float Diff { get; set; }

        public float Fee { get; set; }
    }
}
