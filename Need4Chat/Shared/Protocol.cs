using System;

namespace Need4Chat.Shared
{
    public class ChatMessage
    {
        public string Username { get; set; } = "tester-ChatMessage";
        public string Body { get; set; } = "";
        public bool Mine { get; set; } = false;
        public string ID { get; set; } = string.Empty;
        public DateTime DateAndTime { get; set; } = DateTime.Now;
    }


    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}