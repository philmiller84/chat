using System;
using System.Collections.Generic;

namespace Need4Chat.Server.Models
{
    public partial class User
    {
        public DateTime LastLogin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordHint { get; set; }
        public int Id { get; set; }
    }
}
