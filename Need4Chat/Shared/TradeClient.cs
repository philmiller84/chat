using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Need4Chat.Shared
{

    /// <summary>
    /// Generic client class that interfaces .NET Standard/Blazor with SignalR Javascript client
    /// </summary>
    public class TradeClient : ITradeClient, IAsyncDisposable
    {
        public const string HUBURL = "/TradeHub";

        private readonly string _hubUrl;
        private HubConnection _hubConnection;

        /// <summary>
        /// Ctor: create a new client for the given hub URL
        /// </summary>
        /// <param name="siteUrl">The base URL for the site, e.g. https://localhost:1234 </param>
        /// <remarks>
        /// Changed client to accept just the base server URL so any client can use it, including ConsoleApp!
        /// </remarks>
        public TradeClient(string username, string siteUrl)
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
                _hubConnection = new HubConnectionBuilder().WithUrl(_hubUrl).Build();
                Console.WriteLine("TradeClient: calling Start()");

                // add handler for receiving messages
                _hubConnection.On<ItemDetails, UserInfo>("UpdateItemOffsetForUser", (item, user) => { UpdateItemOffsetForUser(item, user); });
                _hubConnection.On<IEnumerable<ItemDetails>>("BroadcastChanges", (itemChanges) => { BroadcastChanges(itemChanges); });

                // start the connection
                await _hubConnection.StartAsync();

                Console.WriteLine("TradeClient: Start returned");
                _started = true;

                // register user on hub to let other clients know they've joined
                //await _hubConnection.SendAsync("Register", _username);
            }
        }

        public Task RequestNewTradeId(ref int tradeId)
        {
            tradeId = 110;
            return Task.CompletedTask;
        }

        private void BulkHandleReceiveMessages(IEnumerable<TradeMessage> tradeMessages)
        {
            //BulkMessagesReceived?.Invoke(this, new BulkMessagesReceivedEventArgs(tradeMessages));
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="method">event name</param>
        /// <param name="message">message content</param>
        private void HandleReceiveMessage(TradeMessage tradeMessage)
        {
            // raise an event to subscribers
            //MessageReceived?.Invoke(this, new MessageReceivedEventArgs(tradeMessage));
        }

        /// <summary>
        /// Event raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        //public event MessageReceivedEventHandler MessageReceived;
        public event ItemOffsetChangeReceivedEventHandler ItemOffsetChangeReceived;


        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="message">message to send</param>
        public async Task SendAsyncObject(string method, object message)
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

                //await _hubConnection.SendAsync("SendTradeMessage", jsonString);
                await _hubConnection.SendAsync(method, message);
            }
            catch
            {
                throw new Exception("Send to hub failed");
            }
        }


        /// <summary>
        /// Send a message to the hub to add a new item
        /// </summary>
        /// <param name="item">item to add</param>
        public async Task AddNewItem(ItemDetails item)
        {
            await SendAsyncObject("AddNewItem", item);
        }

        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="message">message to send</param>
        public async Task SendAsync(TradeMessage message)
        {
            await SendAsyncObject("SendTradeMessage", message);
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
            Console.WriteLine("TradeClient: Disposing");
            await StopAsync();
        }

        public Task UpdateItemOffsetForUser(ItemDetails item, UserInfo user)
        {
            
            //ItemOffsetChangeReceived?.Invoke(this, new ItemOffsetChangeReceivedEventArgs(tradeMessages));
            throw new NotImplementedException();
        }

        public Task BroadcastChanges(IEnumerable<ItemDetails> itemChanges)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    //public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
    public delegate void ItemOffsetChangeReceivedEventHandler(object sender, ItemOffsetChangeReceivedEventArgs e);

    /// <summary>
    /// Message received argument class
    /// </summary>
    public class ItemOffsetChangeReceivedEventArgs : EventArgs
    {
        //public List<MessageReceivedEventArgs> messageEventArgs = null;

        public ItemOffsetChangeReceivedEventArgs(IEnumerable<TradeMessage> tradeMessages)
        {
            //messageEventArgs = new List<MessageReceivedEventArgs>();

            foreach (var c in tradeMessages)
            {
                //messageEventArgs.Add(new MessageReceivedEventArgs(c));
            }
        }
    }

    //public class MessageReceivedEventArgs : EventArgs
    //{
    //    public MessageReceivedEventArgs(TradeMessage tradeMessage)
    //    {
    //        Username = tradeMessage.Username;
    //        Message = tradeMessage.Body;
    //        Timestamp = tradeMessage.DateAndTime;
    //        ID = tradeMessage.ID;
    //    }

    //    public string ID { get; set; }
    //    public string Username { get; set; }
    //    public string Message { get; set; }
    //    public DateTime Timestamp { get; set; }

    //}

}

