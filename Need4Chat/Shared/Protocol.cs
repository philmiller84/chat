using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Need4Chat.Shared
{

    public interface IChatClient
    {
        Task LogMessageToUser(String username, string message);
        Task BroadcastBulk(IEnumerable<ChatMessage> messages);
        Task BroadcastMessage(ChatMessage message);
        Task SendAsync(ChatMessage message);
    }

    public class ChatMessage
    {
        public string Username { get; set; } = "tester-ChatMessage";
        public string Body { get; set; } = "";
        public bool Mine { get; set; } = false;
        public string ID { get; set; } = string.Empty; // this is the GUID
        public DateTime DateAndTime { get; set; } = DateTime.Now;
    }

    public class ItemDetails
    {
        public string ID { get; set; } = string.Empty; // this is the GUID
        public string description { get; set; } = string.Empty;
        public decimal cost { get; set; }
        public int userItemOffset { get; set; } = 0;
    }

    public interface ILoginClient
    {
        Task ReceiveMessage(string username, string message);
        Task RegisterUserLogin(string username, string message);
        Task SendAsync(LoginInfo userLoginInfo);
    }

    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserInfo
    {
        public string Username { get; set; }
        public string ID { get; set; }
    }

    public class TradeDetails
    {
        public Dictionary<UserInfo, List<ItemDetails>> userItemMap { get; set; }
    }


    public class TradeMessage
    {

    }
}