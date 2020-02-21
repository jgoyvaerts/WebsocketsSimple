# **[WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple)**
[WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) provides an easy-to-use and customizable Websocket Server and Websocket Client. The server is created using [Kestrel Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.1). The server and client can be used for non-SSL or SSL connections and authentication is provided for identifying the clients connected to your server. Both client and server are created in .NET Core 2.1 and use async await functionality. If you are not familiar with async await functionality, you can learn more by reviewing the information found at the [Microsoft Async Programming Concepts page](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/). All [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) packages referenced in this documentation are available on the NuGet package manager at in 1 aggregate package - [WebsocketsSimple](https://www.nuget.org/packages/websocketssimple/).

![Image of WebsocketsSimple Logo](https://pixelhorrorstudios.s3-us-west-2.amazonaws.com/Packages/WebsocketsSimple.png)

* [IPacket](#ipacket)
* [Client](#client)
    * [IWebsocketClient](#iwebsocketclient)
* [Server](#server)
    * [IWebsocketServer](#iwebsocketserver)
    * [IWebsocktServerAuth<T>](#iwebsocketserverauth<T>)
* [Additional Information](#additional-information)

***

## **IPacket**

**IPacket** is an interface contained in [PHS.Networking](https://www.nuget.org/packages/PHS.Networking/) that represents an abstract payload to be sent across Websocket. **IPacket** also includes a default implementation struct, **Packet**, which contains the following information:
* **Data** - *string* - Property representing the payload of the packet, many times this could be JSON or XML that can be deserialized back into an object by the server or other Websocket clients.
* **Timestamp** - *datetime* - Property containing the UTC DateTime when the **Packet** was created / sent to the server.

***

## **Client**

A Websocket Client module is included which can be used for non-SSL or SSL connections. To get started, first install the NuGet package using the NuGet package manager:
> install-package WebsocketsSimple.Client

This will add the most-recent version of the [WebsocketsSimple Client](https://www.nuget.org/packages/WebsocketsSimple.Client/) module to your specified project. 
***
### IWebsocketClient
Once installed, we can create an instance of **IWebsocketClient** with the included implementation **WebsocketClient**. 
* `WebsocketClient(IParamsWSClient parameters, string oauthToken = "")`
    * An example instantiation is below:

            IWebsocketClient client = new WebsocketCient(new ParamsWSClient
            {
                Uri = "connect.websocketssimple.com",
                Port = 8989,
                IsSSL = false
            });

* There is an optional parameter on the constructor called **oauthToken** used by the [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) for authenticating a user.
    * An example instantiation is below:

            IWebsocketClient client = new WebsocketClient(new ParamsWSClient
            {
                 Uri = "connect.websocketssimple.com",
                 Port = 8989,
                 IsSSL = false
            }, "oauthToken");
    * Generating and persisting identity **OAuth tokens** is outside the scope of this tutorial, but for more information, check out [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) for a robust and easy-to-use .NET identity server.
    
#### Parameters
* **IParamsWSClient** - *Required* - [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) includes a default struct implementation called **ParamsWSClient** which contains the following connection detail data:

    * **Uri** - *string* - The endpoint / host / url of the Websocket Server instance to connect (e.g. localhost, 192.168.1.14, [connect.websocketssimple.com](#).
    * **Port** - *int* - The port of the Websocket Server instance to connect (e.g. 6660, 7210, 6483).
    * **IsSSL** - *bool* - Flag specifying if the connection should be made using SSL encryption for the connection to the server.
* **OAuth Token** - *Optional* - Optional parameter used by the [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) for authenticating a user.

#### Events
3 events are exposed on the **IWebsocketClient** interface: `MessageEvent`, `ConnectionEvent`, and `ErrorEvent`. These event signatures are below:

        client.MessageEvent += OMessageEvent;
        client.ConnectionEvent += OnConnectionEvent;
        client.ErrorEvent += OnErrorEvent

* `Task OnMessageEvent(object sender, WSMessageClientEventArgs args);`
    * Invoked when a message is sent or received
* `Task OnConnectionEvent(object sender, WSConnectionClientEventArgs args);`
    * Invoked when the [WebsocketsSimple Client](https://www.nuget.org/packages/WebsocketsSimple.Client/) is connecting, connects, or disconnects from the server
* `Task OnErrorEvent(object sender, WSErrorClientEventArgs args);`
    * Wraps all internal logic with try catch statements and outputs the specific error(s)

#### Connect to a Websocket Server
To connect to a Websocket Server, invoke the function `ConnectAsync()`.

        await client.ConnectAsync());
        
*Note: Connection parameters were input with the constructors of **WebsocketClient**.*

#### SSL
To enable SSL for [WebsocketsSimple Client](https://www.nuget.org/packages/WebsocketsSimple.Client/), set the **IsSSL** flag in **IParamsWSClient** to true. In order to connect successfully, the server must have a valid, non-expired SSL certificate where the certificate's issued hostname must match the Uri specified in **IParamsWSClient**. For example, the Uri in the above examples is [connect.websocketssimple.com](#), and the SSL certificate on the server must be issued to [connect.websocketssimple.com](#).

*Please note that a self-signed certificate or one from a non-trusted CA is not considered a valid SSL certificate.*

#### Send a Message to the Server
3 functions are exposed to send messages to the server:
* `SendToServerAsync<T>(T packet) where T : IPacket`
    * Send the designated packet to the server 
* `SendToServerAsync(string message)`
    * Transform the message into a **Packet** and send to the server 
* `SendToServerRawAsync(string message)`
	* Send the message directly to the server without transforming into a **Packet**. 

An example call to send a message to the server could be:

        await client.SendToServerAsync<IPacket>(new Packet 
        {
            Data = "YourDataPayload",
            DateTime = 2020-02-18 11:54:32.4324
        });
        
More information about **IPacket** is available [here](#ipacket).

#### OAuth Token
If you are using **WebsocketClient**, an optional parameter is included in the constructor for your **OAuth Token** - for more information, see **[IWebsocketClient](#iwebsocketclient)**. However, if you are creating a manual Websocket connection to an instance of **`WebsocketServerAuth<T>`**, you must append your **OAuth Token** to your connection Uri. This could look similar to the following:
    
>wss://connect.websocketssimple.com/oauthtoken

#### Extending IPacket
**IPacket** can be extended with additional datatypes into a new struct / class and passed into the generic `SendToServerAsync<T>(T packet) where T : IPacket` function. Please note that **Packet** is a struct and cannot be inherited - please instead implement the interface **IPacket**.

        enum PacketExtendedType
        {
            PacketType1,
            PacketType2
        }

        interface IPacketExtended : IPacket 
        {
            string Username { get; set; }
            string FirstName { get; set; }
            string LastName { get; set; }
        }

        public class PacketExtended : IPacket 
        {
            string Data { get; set; }
            DateTime Timestamp { get; set; }
            PacketExtendedType PacketExtendedType {get; set; }
            string Username { get; set; }
            string FirstName { get; set; }
            string LastName { get; set; }
        }

        ---

        await SendToServerAsync<IPacketExtended>(new PacketExtended 
        {
            Data = "YourDataPayload",
            DateTime = 2020-02-12 01:29:23.963,
            FirstName = "FakeFirstName",
            LastName = "FakeLastName",
            PacketExtendedType = PacketExtendedType.PacketType1
        });

#### Receiving an Extended IPacket
If you want to extend **IPacket** to include additional fields, you will need to override the **WebsocketClient** implementation to support the extended type. First define a new class that inherits **WebsocketCient**, override the protected method called `MessageReceived(string message, IConnectionServer connection)`, and deserialize into the extended **IPacket** of your choice. An example of this implementation is below:

    public class WebsocketClientExtended : WebsocketClient
    {
        public WebsocketClientExtended(IParamsWSClient parameters, string oauthToken = "")
        {
        }

        protected override IPacket MessageReceived(string message, IConnectionServer connection)
        {
            IPacket packet;

            try
            {
                packet = JsonConvert.DeserializeObject<PacketExtended>(message);

                if (string.IsNullOrWhiteSpace(packet.Data))
                {
                    packet = new PacketExtended
                    {
                        Data = message,
                        Timestamp = DateTime.UtcNow,
                        PacketExtendedType = PacketExtendedType.PacketType1
                    };
                }
            }
            catch
            {
                packet = new PacketExtended
                {
                    Data = message,
                    Timestamp = DateTime.UtcNow,
                    PacketExtendedType = PacketExtendedType.PacketType1
                };
            }

            return packet;
        }
    }

If you don’t know the type of an object ahead of time, first deserialize the message into a class or struct that contains “common” fields, such as PacketExtended with a PacketExtendedType enum field. Then use the value of PacketExtendedType and deserialize a second time into that type. Repeat until the your custom object is completely deserialized.

#### Ping
A **WebsocketServer** will send a raw message containing **'ping'** to every client every 120 seconds to verify which connections are still alive. If a client fails to respond with a raw message containing **'pong'**, during the the next ping cycle, the connection will be severed and disposed. However, if you are using **WebsocketClient**, the ping / pong messages are digested and handled before reaching `MessageEvent(object sender, WSMessageServerEventArgs args)`. This means you do not need to worry about ping and pong messages if you are using **WebsocketClient**. However, if you are creating your own Websocket connection, you should incorporate logic to listen for raw messages containing **'ping'**, and if received, immediately respond with a raw message containing **'pong'** message. 

*Note: Failure to implement this logic will result in a connection being disconnected and disposed in up to approximately 240 seconds.*

#### Disconnect from the Server
To disconnect from the server, invoke the function `Disconnect()`.

#### Disposal
At the end of usage, be sure to call `Dispose()` on the **IWebsocketClient** to free all allocated memory and resources.
***
## **Server**
A Websocket Server module is included which can be used for non-SSL or SSL connections. The server requires a [Kestrel Server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.1) to handle the initial Websocket connection, and the easiest way to create one is to create an Asp.NET Core WebApp. You can also create a [Kestrel Server manually](https://jonhilton.net/2016/07/18/your-first-net-core-web-application-using-nothing-but-the-command-line/). Once your project is setup, install the NuGet package using the NuGet package manager:
> install-package WebsocketsSimple.Server

This will add the most-recent version of the [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) module to your specified project. 

Once installed, we can create 2 different classes of Websocket Servers. 

* **[`IWebsocketServer`](#iwebsocketserver)**
* **[`IWebsocketServerAuth<T>`](#iwebsocketserverauth<T>)**
***
### `IWebsocketServer`
We will now create an instance of **IWebsocketServer** with the included implementation **WebsocketServer**. The included implementation includes the following constructor:

* `WebsocketServer(IParamsWSServer parameters, WebsocketHandler handler = null)`
    * The constructor for a non-SSL Websocket Server. Example instantiation is below:
 
            IWebsocketServer server = new WebsocketServer(new ParamsWSServer 
            {
                Port = 8989,
                EndOfLineCharacters = "\r\n",
                ConnectionSuccessString = "Connected Successfully",
                IsWebsocketSecured = true
            });

The [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) does not specify a listening uri / host. Instead, the server is configured to automatically listen on all available interfaces (including 127.0.0.1, localhost, and the server's exposed IPs).

#### Parameters
* **IParamsWSServer** - *Required* - [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) includes a default struct implementation called **ParamsWSServer** which contains the following connection detail data:
    * **Port** - *int* - The port where the Websocket server will listen (e.g. 6660, 7210, 6483).
    * **ConnectionSuccessString** - *string* - The string that will be sent to a newly connected client.
    * **IsWebsocketSecured** - *bool* - Flag to indicate if the Websocket server is using a valid SSL certificate to encrypt traffic or if the Websocket server is unsecured. SSL is secured via your webserver (e.g. Apache, IIS, Nginx, etc.).
* **WebsocketHandler** - *Optional* - If you want to deserialize an extended **IPacket** from a client, you can extend **WebsocketHandler** in a new class and override `MessageReceived(string message, IConnectionServer connection)` to deserialize the object into the class / struct of your choice. For more information, please see **[Receiving an Extended IPacket](#receiving-an-extended-ipacket)** below.

#### Events
4 events are exposed on the **IWebsocketServer** interface: `MessageEvent`, `ConnectionEvent`, `ErrorEvent`, and `ServerEvent`. These event signatures are below:

        server.MessageEvent += OMessageEvent;
        server.ConnectionEvent += OnConnectionEvent;
        server.ErrorEvent += OnErrorEvent;
        server.ServerEvent += OnServerEvent;

* `Task OnMessageEvent(object sender, WSMessageServerEventArgs args);`
    * Invoked when a message is sent or received
* `Task OnConnectionEvent(object sender, WSConnectionServerEventArgs args);`
    * Invoked when a Websocket client is connecting, connects, or disconnects from the server
* `Task OnErrorEvent(object sender, WSErrorServerEventArgs args);`
    * Wraps all internal logic with try catch statements and outputs the specific error(s)
* `Task OnServerEvent(object sender, ServerEventArgs args);`
    * Invoked when the Websocket server starts or stops

#### Starting the Websocket Server
There is no action to start the [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) - once instantiated, the server will listen on the specified port until disposed.

#### SSL
To enable SSL for [WebsocketsSimple Server](https://www.nuget.org/packages/WebsocketsSimple.Server/), use one of the two provided SSL server constructors and manually specify the SSL certificate or direct the parameters to your Windows Certificate Store. In order to allow successful SSL connections, you must have a valid, non-expired SSL certificate. There are many sources for SSL certificates and some of them are opensource community driven - we recommend [Let's Encrypt](https://letsencrypt.org/).

*Note: A self-signed certificate or one from a non-trusted CA is not considered a valid SSL certificate.*

#### Send a Message to a Client
3 functions are exposed to send messages to clients: 
* `SendToConnectionAsync<T>(T packet, IConnectionServer connection) where T : IPacket`
    * Send the designated **IPacket** to the specified connection
* `SendToConnectionAsync(string message, IConnectionServer connection)`
    * Transform the message into a **Packet** and send to the specified connection
* `SendToConnectionRawAsync(string message, IConnectionServer connection)`
    * Send the message to the specified connection directly without transforming it into a **Packet**

More information about **IPacket** is available [here](#ipacket).

**IConnectionServer** is a connncted client to the server. These are exposed in the `ConnectionEvent` or can be retrieved from the **Connections** inside of **IWebsocketServer**.

An example call to send a message to a client could be:

        IConnectionServer[] connections = server.Connections;

        await server.SendToConnectionAsync<IPacket>(new Packet 
        {
            Data = "YourDataPayload",
            DateTime = 2020-02-18 11:54:32.4324
        }, connections[0]);

#### Receiving an Extended IPacket
If you want to extend **IPacket** to include additional fields, you will need to add the optional parameter **WebsocketHandler** that can be included with each constructor. The included **WebsocketHandler** has logic which is specific to deserialize messages of type **Packet**, but to receive your own extended **IPacket**, we will need to inherit / extend **WebsocketHandler** with our your class. Once **WebsocketHandler** has been extended, override the protected method called `MessageReceived(string message, IConnectionServer connection)` and deserialize into the extended **IPacket** of your choice. An example of this implementation is below:

    public class WebsocketHandlerExtended : WebsocketHandler
    {
        public WebsocketHandlerExtended(IParamsWSServer parameters) : base(parameters)
        {
        }

        protected override IPacket MessageReceived(string message, IConnectionServer connection)
        {
            IPacket packet;

            try
            {
                packet = JsonConvert.DeserializeObject<PacketExtended>(message);

                if (string.IsNullOrWhiteSpace(packet.Data))
                {
                    packet = new PacketExtended
                    {
                        Data = message,
                        Timestamp = DateTime.UtcNow,
                        PacketExtendedType = PacketExtendedType.PacketType1
                    };
                }
            }
            catch
            {
                packet = new PacketExtended
                {
                    Data = message,
                    Timestamp = DateTime.UtcNow,
                    PacketExtendedType = PacketExtendedType.PacketType1
                };
            }

            return packet;
        }
    }

If you don’t know the type of an object ahead of time, first deserialize the message into a class or struct that contains “common” fields, such as PacketExtended with a PacketExtendedType enum field. Then use the value of PacketExtendedType and deserialize a second time into that type. Repeat until the your custom object is completely deserialized.

Finally, when constructing your **IWebsocketServer**, pass in your new **WebsocketHandler** extended class you created. An example is as follows:

    IParamsWSServer parameters = new ParamsWSServer 
    {
        ConnectionSuccessString = "Connected Successfully"
    };

    IWebsocketServer server = new WebsocketServer(parameters, handler: new WebsocketHandlerExtended(parameters, cert));

#### Ping
A raw message containing **'ping'** is sent automatically every 120 seconds to each client connected to a **WebsocketServer**. Each client is expected to immediately return a raw message containing **'pong'**. If a raw message containing **'pong'** is not received by the server before the next ping interval, the connection will be disconnected and removed from the **WebsocketServer**. This interval time is hard-coded to 120 seconds.

#### Disconnect a Client
To disconnect from the server, invoke the function `DisconnectConnection(IConnectionServer connection)`. **IConnectionServer** is a connected client to the server. These are exposed in the `ConnectionEvent` or can be retrieved from the **Connections** inside of **IWebsocketServer**.

#### Stop the Server and Disposal
To stop the server, call the `Dispose()` method to stop listening and free all allocated memory and resources.

***
### `IWebsocketServerAuth<T>`
The second Websocket Server available is slightly more complex but includes authentication for identifying your connections. We will create an instance of **`IWebsocketServerAuth<T>`**  with the included implementation **`WebsocketServerAuth<T>`**. This object includes a generic, T, which represents the datatype of your user unique Id. For example, T could be an int, a string, a long, or a guid - this depends on the datatype of the unique Id you have set for your user. This generic allows the **`IWebsocketServerAuth<T>`** implementation to allow authentication and identification of users within many different user systems. The included implementation includes the following constructor:

* `WebsocketServerAuth<T>(IParamsWSServerAuth parameters, IUserService<T> userService, WebsocketHandler handler = null)`
    * The constructor for a Websocket server. Example instantiation is below:
 
            public class MockUserService : IUserService<long> 
            { }

            IWebsocketServerAuth<long> server = new WebsocketServerAuth<long>(new ParamsWSServerAuth 
            {
                ConnectionSuccessString = "Connected Successfully",
                ConnectionUnauthorizedString = "Connection not authorized",
            }, new MockUserService());

The [WebsocketsSimple Authentication Server](https://www.nuget.org/packages/WebsocketsSimple.Server/) does not specify a listening uri / host. Instead, the server is configured to listen on Uri exposed by your webserver and on the port specified in the parameters.

#### Parameters
* **IParamsWSServerAuth** - *Required*. [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple)) includes a default struct implementation called **ParamsWSServerAuth** which contains the following connection detail data:
    * **ConnectionSuccessString** - *string* - The string that will be sent to a newly connected client.
    * **ConnectionUnauthorizedString** - *string* - The string that will be sent to a connected client when they fail authentication.
* **`IUserService<T>`** - *Required* - This is an interface for a UserService class that will need to be implemented. This interface specifies 1 function, `GetIdAsync(string token)`, which will be invoked when the server receives an **OAuth Token** from a new connection. For more information regarding the User Service class, please see **[`IUserService<T>`](#userservice<T>)** below.
* **WebsocketHandler** - *Optional*. This is an object that can be passed in optionally. If you want to deserialize an extended **IPacket** from a client, you would extend **WebsocketHandler** in a new class and override `MessageReceived(string message, IConnectionServer connection)` to deserialize the object into the class / struct of your choice. For more information, please see **[Receiving an Extended IPacket](#receiving-an-extended-ipacket)** below.

#### `IUserService<T>`
This is an interface contained in [PHS.Networking.Server](https://www.nuget.org/packages/PHS.Networking.Server/). When creating a **`WebsocketServerAuth<T>`**, this inteface **`IUserService<T>`** will need to be instantiated into a concrete class. A default implementation is *not* included with [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple). An example implementation is shown below:

    public class UserServiceImplementation : IUserService<long>
    {
        protected readonly ApplicationDbContext _ctx;

        public UserServiceWS(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public virtual async Task<long> GetIdAsync(string token)
        {
            // Obfuscate the token in the database
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            var user = await _ctx.Users.FirstOrDefaultAsync(s => s.OAuthToken == token);
            return user != null ? user.Id : (default);
        }

        public void Dispose()
        {
        }


Because you are responsible for filling the logic in `GetIdAsync(string oauthToken)`, the data could reside in many stores including (but not limited to) in memory, in a database, or against an identity server. In our implementation, we are checking the **OAuth Token** using [Entity Framework](https://docs.microsoft.com/en-us/ef/) and checking it against a quick User table. If the **OAuth Token** is found, then the appropriate UserId will be returned as type T, and if not, the default of type T will be returned (e.g. 0, "", Guid.Empty).

#### Events
4 events are exposed on the **`IWebsocketServerAuth<T>`** interface: MessageEvent, ConnectionEvent, ErrorEvent, and ServerEvent. These event signatures are below:

        server.MessageEvent += OMessageEvent;
        server.ConnectionEvent += OnConnectionEvent;
        server.ErrorEvent += OnErrorEvent;
        server.ServerEvent += OnServerEvent;

* `Task OnMessageEvent(object sender, WSMessageServerAuthEventArgs<T> args);`
    * Invoked when a message is sent or received
* `Task OnConnectionEvent(object sender, WSConnectionServerAuthEventArgs<T> args);`
    * Invoked when a Websocket client is connecting, connects, or disconnects from the server
* `Task OnErrorEvent(object sender, WSErrorServerAuthEventArgs<T> args);`
    * Wraps all internal logic with try catch statements and outputs the specific error(s)
* `Task OnServerEvent(object sender, ServerEventArgs args);`
    * Invoked when the Websocket server starts or stops

#### Start the Websocket Authentication Server
There is no action to start the Websocket Server - once instantiated and added to the [Kestrel Middleware Pipeline](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.1), the server will listen on the specified port until the Kestrel Server is terminated.

#### SSL
To enable SSL for [WebsocketsSimple Authentication Server](https://www.nuget.org/packages/WebsocketsSimple.Server/), bind a SSL server to the listening port on specified on your Webserver (e.g. Apache, IIS, Nginx, etc). In order to allow successful connections, the server must have a valid, non-expired SSL certificate. Please note that a self-signed certificate or one from a non-trusted CA is not considered a valid SSL certificate. There are many sources for SSL certificates - we recommend [Let's Encrypt](https://letsencrypt.org/).

#### Send a Message to a Client
To send messages to a client, 11 functions are exposed:
* `BroadcastToAllAuthorizedUsersAsync<S>(S packet) where S : IPacket`
    * Send the designated packet to all Users and their connections currently logged into the server
* `BroadcastToAllAuthorizedUsersAsync(string message)`
    * Transform the message into a **Packet** and send to all Users and their connections currently logged into the server
* `BroadcastToAllAuthorizedUsersAsync(S packet, IConnectionServer connectionSending) where S : IPacket`
    * Send the designated packet to all Users and their connections currently logged into the server except for the connection matching connectionSending
* `BroadcastToAllAuthorizedUsersAsync(string message, IConnectionServer connectionSending)`
    * Transform the message into a **Packet** and send to all Users and their connections currently logged into the server except for the connection matching the connectionSending
* `BroadcastToAllAuthorizedUsersRawAsync(string message)`
    * Send the message directly to all Users and their connections currently logged into the server without transforming the message into a **Packet**
* `SendToUserAsync<S>(S packet, T userId) where S : IPacket`
    * Send the designated packet to the specified User and their connections currently logged into the server
* `SendToUserAsync(string message, T userId)`
    * Transform the message into a **Packet** and send the to the specified User and their connections currently logged into the server
* `SendToUserRawAsync(string message, T userId)`
    * Send the message directly to the designated User and their connections without transforming the message into a **Packet**
* `SendToConnectionAsync<S>(S packet, IConnectionServer connection) where S : IPacket`
    * Send the designated packet to the designated User's connection currently logged into the server
* `SendToConnectionAsync(string message, IConnectionServer connection)`
    * Transform the message into a **Packet** and send to the designated User's connection currently logged into the server
* `SendToConnectionRawAsync(string message, IConnectionServer connection`
    * Send the message directly to the designated User's connection currently logged into the server without transforming the message into a **Packet**

More information about **IPacket** is available [here](#ipacket).

**IConnectionServer** is a connected client to the server. These are exposed in the `ConnectionEvent` or can be retrieved from **UserConnections** and then **Connections** inside of **`IWebsocketServerAuth<T>`**.

An example call to send a message to a client could be:

        IUserConnection<T>[] userConnections = server.UserConnections;

        await server.SendToConnectionAsync<IPacket>(new Packet 
        {
            Data = "YourDataPayload",
            DateTime = 2020-02-18 11:54:32.4324
        }, userConnections[0].Connections[0]);

#### Receiving an Extended IPacket
If you want to extend **IPacket** to include additional fields, you will need to add the optional parameter **WebsocketHandler** that can be included with each constructor. The included **WebsocketHandler** has logic which is specific to deserialize messages of type **Packet**, but to receive your own extended **IPacket**, we will need to inherit / extend **WebsocketHandler** with your own class. Once **WebsocketHandler** has been extended, override the protected method called `MessageReceived(string message, IConnectionServer connection)` and deserialize into the extended **IPacket** of your choice. An example of this implementation is below:

    public class WebsocketHandlerExtended : WebsocketHandler
    {
        public WebsocketHandlerExtended(IParamsWSServer parameters) : base(parameters)
        {
        }

        protected override IPacket MessageReceived(string message, IConnectionServer connection)
        {
            IPacket packet;

            try
            {
                packet = JsonConvert.DeserializeObject<PacketExtended>(message);

                if (string.IsNullOrWhiteSpace(packet.Data))
                {
                    packet = new PacketExtended
                    {
                        Data = message,
                        Timestamp = DateTime.UtcNow,
                        PacketExtendedType = PacketExtendedType.PacketType1
                    };
                }
            }
            catch
            {
                packet = new PacketExtended
                {
                    Data = message,
                    Timestamp = DateTime.UtcNow,
                    PacketExtendedType = PacketExtendedType.PacketType1
                };
            }

            return packet;
        }
    }

If you don’t know the type of an object ahead of time, first deserialize the message into a class or struct that contains “common” fields, such as PacketExtended with a PacketExtendedType enum field. Then use the value of PacketExtendedType and deserialize a second time into that type. Repeat until the your custom object is completely deserialized.

Finally, when constructing **`WebsocketServerAuth<T>`**, pass in your new **WebsocketHandler** extended class you created. An example is as follows:

    IParamsWSServerAuth parameters = new ParamsTWSServerAuth 
    {
        ConnectionSuccessString = "Connected Successfully",
        ConnectionUnauthorizedString = "Connection Not Authorized"
    };

    IWebsocketServerAuth<long> server = new WebsocketServerAuth<long>(parameters, new MockUserService(), handler: new WebsocketHandlerExtended(parameters));

#### Ping
A raw message containing **'ping'** is sent automatically every 120 seconds to each client connected to a **`WebsocketServerAuth<T>`**. Each client is expected to return a raw message containing a **'pong'**. If a 'pong' is not received before the next ping interval, the connection will be disconnected and removed from the **`WebsocketServerAuth<T>`**. This interval time is hard-coded to 120 seconds.

#### Disconnect a Client
To disconnect from the server, invoke the function `DisconnectConnection(IConnectionServer connection)`. 

**IConnectionServer** is a connected client to the server. These are exposed in the `ConnectionEvent` or can be retrieved from **UserConnnections** then **Connections** inside of **`IWebsocketServerAuth<T>`**. If a logged in User disconnects from all connections, that user is automatically removed from **UserConnections**.

#### Stop the Server and Disposal
To stop the server, call the `Dispose()` method to stop listening and free all allocated memory and resources.

***

### Additional Information
[WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) was created by [LiveOrDevTrying](https://www.liveordevtrying.com) and is maintained by [Pixel Horror Studios](https://www.pixelhorrorstudios.com). [WebsocketsSimple](https://www.github.com/liveordevtrying/websocketssimple) is currently implemented in (but not limited to) the following projects: [Allie.Chat](https://allie.chat) and [The Monitaur](https://www.themonitaur.com).  
![Pixel Horror Studios Logo](https://pixelhorrorstudios.s3-us-west-2.amazonaws.com/Packages/PHS.png)