using Microsoft.AspNetCore.SignalR.Client;
using Need4Chat.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Need4Chat.Shared
{

    /// <summary>
    /// Generic client class that interfaces .NET Standard/Blazor with SignalR Javascript client
    /// </summary>
    public class ChatClient : IChatClient, IAsyncDisposable
    {
        public const string HUBURL = "/ChatHub";

        private readonly string _hubUrl;
        private HubConnection _hubConnection;

        /// <summary>
        /// Ctor: create a new client for the given hub URL
        /// </summary>
        /// <param name="siteUrl">The base URL for the site, e.g. https://localhost:1234 </param>
        /// <remarks>
        /// Changed client to accept just the base server URL so any client can use it, including ConsoleApp!
        /// </remarks>

        public ChatClient(string username, string siteUrl)

        {

            // check inputs
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(siteUrl))
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }
            // save username
            _username = username;
            // set the hub URL
            _hubUrl = siteUrl.TrimEnd('/') + HUBURL;
        }

        /// <summary>
        /// Name of the chatter
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// Flag to show if started
        /// </summary>
        private bool _started = false;

        /// <summary>
        /// Start the SignalR client 
        /// </summary>
        public async Task StartAsync()
        {
            if (!_started)
            {
                // create the connection using the .NET SignalR client
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .Build();
                Console.WriteLine("ChatClient: calling Start()");

                _hubConnection.On<string, string>("LogMessageToUser", (user, message) => { LogMessageToUser(user, message); });
                _hubConnection.On<IEnumerable<ChatMessage>>("BroadcastBulk", (chatMessages) => { BroadcastBulk(chatMessages); });
                _hubConnection.On<ChatMessage>("BroadcastMessage", (chatMessage) => { BroadcastMessage(chatMessage); });

                // start the connection
                await _hubConnection.StartAsync();

                Console.WriteLine("ChatClient: Start returned");
                _started = true;

                // register user on hub to let other clients know they've joined
                await _hubConnection.SendAsync("Register", _username);
            }
        }

        public Task LogMessageToUser(string username, string message)
        {
            HandleReceiveMessage(new ChatMessage() { Username = username, Body = message });
            return Task.CompletedTask;
        }

        public Task BroadcastBulk(IEnumerable<ChatMessage> messages)
        {
            BulkHandleReceiveMessages(messages);
            return Task.CompletedTask;
        }
        public Task BroadcastMessage(ChatMessage message)
        {
            HandleReceiveMessage(message);
            return Task.CompletedTask;
        }

        private void BulkHandleReceiveMessages(IEnumerable<ChatMessage> chatMessages)
        {
            BulkMessagesReceived?.Invoke(this, new BulkMessagesReceivedEventArgs(chatMessages));
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="method">event name</param>
        /// <param name="message">message content</param>
        private void HandleReceiveMessage(ChatMessage chatMessage)
        {
            // raise an event to subscribers
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(chatMessage));
        }

        /// <summary>
        /// Event raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        public event MessageReceivedEventHandler MessageReceived;
        public event BulkMessagesReceivedEventHandler BulkMessagesReceived;

        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="message">message to send</param>
        public async Task SendAsync(ChatMessage message)
        {
            // check we are connected
            if (!_started)
            {
                throw new InvalidOperationException("Client not started");
            }
            // send the message
            try
            {
                //string jsonString = JsonSerializer.Serialize(message);

                //await _hubConnection.SendAsync("SendChatMessage", jsonString);
                await _hubConnection.SendAsync("SendChatMessage", message);
            }
            catch
            {
                throw new Exception("Send to hub failed");
            }
        }

        /// <summary>
        /// Stop the client (if started)
        /// </summary>
        public async Task StopAsync()
        {
            if (_started)
            {
                // disconnect the client
                await _hubConnection.StopAsync();
                // There is a bug in the mono/SignalR client that does not
                // close connections even after stop/dispose
                // see https://github.com/mono/mono/issues/18628
                // this means the demo won't show "xxx left the chat" since 
                // the connections are left open
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _started = false;
            }
        }


        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("ChatClient: Disposing");
            await StopAsync();
        }
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
    public delegate void BulkMessagesReceivedEventHandler(object sender, BulkMessagesReceivedEventArgs e);

    /// <summary>
    /// Message received argument class
    /// </summary>
    public class BulkMessagesReceivedEventArgs : EventArgs
    {
        public List<MessageReceivedEventArgs> messageEventArgs = null;

        public BulkMessagesReceivedEventArgs(IEnumerable<ChatMessage> chatMessages)
        {
            messageEventArgs = new List<MessageReceivedEventArgs>();

            foreach (ChatMessage c in chatMessages)
            {
                messageEventArgs.Add(new MessageReceivedEventArgs(c));
            }
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(ChatMessage chatMessage)
        {
            Username = chatMessage.Username;
            Message = chatMessage.Body;
            Timestamp = chatMessage.DateAndTime;
            ID = chatMessage.ID;
        }

        public string ID { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

}

