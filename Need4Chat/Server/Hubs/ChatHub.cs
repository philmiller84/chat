using Microsoft.AspNetCore.SignalR;
using Need4Chat.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Need4Chat.Server.Hubs
{
    /// <summary>
    /// The SignalR hub 
    /// </summary>
    public class ChatHub : Hub<IChatClient>
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
        public async Task SendChatMessage(ChatMessage message)
        {
            try
            {
                bool messageAdded = false;
                await Task.Run(() => messageAdded = dbMiddleware.AddMessage(message));
                if (messageAdded)
                {
                    await Clients.All.BroadcastMessage(message);
                }
                else
                {
                    await Clients.Caller.LogMessageToUser(message.Username, "Failed to send message");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
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
                await Clients.AllExcept(currentId).LogMessageToUser(username, $"{username} joined the chat");
            }
        }

        /// <summary>
        /// Log connection
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");

            Clients.Caller.BroadcastBulk(dbMiddleware.GetMessages());

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
            await Clients.AllExcept(Context.ConnectionId).LogMessageToUser(username, $"{username} has left the chat");
            await base.OnDisconnectedAsync(e);
        }


    }
}
