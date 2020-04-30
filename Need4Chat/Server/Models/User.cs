using System;

namespace Need4Chat.Server.Models
{
    public partial class User
    {
        public Guid Id { get; set; }
        public DateTime LastLogin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordHint { get; set; }
    }
}
