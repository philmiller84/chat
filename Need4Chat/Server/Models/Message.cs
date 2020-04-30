using System;

namespace Need4Chat.Server.Models
{
    public partial class Message
    {
        public Guid Id { get; set; }
        public Guid User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
    }
}
