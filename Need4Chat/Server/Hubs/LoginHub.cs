using Microsoft.AspNetCore.SignalR;
using Need4Chat.Server.Models;
using Need4Chat.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Need4Chat.Server.Hubs
{
    /// <summary>
    /// The SignalR hub 
    /// </summary>
    public class LoginHub : Hub<ILoginClient>
    {
        /// <summary>
        /// connectionId-to-username lookup
        /// </summary>
        /// <remarks>
        /// Needs to be static as the chat is created dynamically a lot
        /// </remarks>
        private static readonly Dictionary<string, string> userLookup = new Dictionary<string, string>();


        private static readonly DbMiddleware dbMiddleware = new DbMiddleware();

        /// <summary>
        /// Send a message to all clients
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendLoginMessage(LoginInfo userLogin)
        {
            User userInfo = new User { Name = userLogin.Username, Password = userLogin.Password };

            if (dbMiddleware.TryLoginAttempt(userInfo))
            {
                string message = "User logged in";
                await Clients.All.RegisterUserLogin(userLogin.Username, message);
            }
            else
            {
                string message = "Attempted login failed";
                await Clients.Caller.RegisterUserLogin(userLogin.Username, message);
            }
        }

        /// <summary>
        /// Register username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task Register(string username)
        {
            string currentId = Context.ConnectionId;
            if (!userLookup.ContainsKey(currentId))
            {
                // maintain a lookup of connectionId-to-username
                userLookup.Add(currentId, username);
                // re-use existing message for now
                //await Clients.AllExcept(currentId).SendAsync(
                //	"ReceiveMessage",
                //	username, $"{username} joined the chat");
            }
            await Clients.AllExcept(currentId).ReceiveMessage(username, $"{username} registered");
        }

        /// <summary>
        /// Log connection
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");

            foreach (User u in dbMiddleware.GetRegisteredUsers())
            {
                Clients.Caller.ReceiveMessage(u.Name, "registered user");
            }

            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Log disconnection
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            // try to get connection
            string id = Context.ConnectionId;
            if (!userLookup.TryGetValue(id, out string username))
            {
                username = "[unknown]";
            }

            userLookup.Remove(id);
            //await Clients.AllExcept(Context.ConnectionId).SendAsync(
            //	"ReceiveMessage",
            //	username, $"{username} has left the chat");
            await base.OnDisconnectedAsync(e);
        }


    }
}
