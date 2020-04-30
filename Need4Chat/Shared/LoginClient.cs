using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Need4Chat.Shared
{

    /// <summary>
    /// Generic client class that interfaces .NET Standard/Blazor with SignalR Javascript client
    /// </summary>
    public class LoginClient : IAsyncDisposable
    {
        public const string HUBURL = "/LoginHub";

        private readonly string _hubUrl;
        private HubConnection _hubConnection;

        /// <summary>
        /// Ctor: create a new client for the given hub URL
        /// </summary>
        /// <param name="siteUrl">The base URL for the site, e.g. https://localhost:1234 </param>
        /// <remarks>
        /// Changed client to accept just the base server URL so any client can use it, including ConsoleApp!
        /// </remarks>

        public LoginClient(string username, string siteUrl)

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
                Console.WriteLine("LoginClient: calling Start()");

                // add handler for receiving messages
                _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    HandleReceiveMessage(user, message);
                });


                _hubConnection.On<string, string>("RegisterUserLogin", (user, message) =>
                {
                    RegisterUserLogin(user, message);
                });

                // start the connection
                await _hubConnection.StartAsync();

                Console.WriteLine("LoginClient: Start returned");
                _started = true;

                // register user on hub to let other clients know they've joined
                await _hubConnection.SendAsync("Register", _username);
            }
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="method">event name</param>
        /// <param name="message">message content</param>
        private void HandleReceiveMessage(string username, string message)
        {
            // raise an event to subscribers
            MessageReceived?.Invoke(this, new LoginReceivedEventArgs(username, "", message));
        }

        /// <summary>
        /// Event raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        public event LoginReceivedEventHandler MessageReceived;
        public event LoginReceivedEventHandler LoginMessageReceived;

        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="message">message to send</param>
        public async Task SendAsync(LoginInfo userLoginInfo)
        {
            // check we are connected
            if (!_started)
            {
                throw new InvalidOperationException("Client not started");
            }

            await _hubConnection.SendAsync("SendLoginMessage", userLoginInfo);
        }


        private void RegisterUserLogin(string username, string message)
        {
            // raise an event to subscribers
            LoginMessageReceived?.Invoke(this, new LoginReceivedEventArgs(username, "", message));
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
            Console.WriteLine("LoginClient: Disposing");
            await StopAsync();
        }
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    public delegate void LoginReceivedEventHandler(object sender, LoginReceivedEventArgs e);

    /// <summary>
    /// Message received argument class
    /// </summary>
    public class LoginReceivedEventArgs : EventArgs
    {
        public LoginReceivedEventArgs(string username, string password, string message)
        {
            Username = username;
            Password = password;
            Message = message;
        }

        /// <summary>
        /// Name of the message/event
        /// </summary>
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Message data items
        /// </summary>
        public string Message { get; set; }

    }

}

