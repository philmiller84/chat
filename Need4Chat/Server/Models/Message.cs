using System;
using System.Collections.Generic;

namespace Need4Chat.Server.Models
{
    public partial class Message
    {
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public int? UserId { get; set; }
        public int Id { get; set; }
    }
}
