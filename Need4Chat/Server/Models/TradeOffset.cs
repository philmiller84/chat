using System;
using System.Collections.Generic;

namespace Need4Chat.Server.Models
{
    public partial class TradeOffset
    {
        public int TradeRequirementId { get; set; }
        public int UserId { get; set; }
        public int? Offset { get; set; }
    }
}
