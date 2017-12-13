# Unicorn.Router Usage
This section describes the router usage in general.

## Startup & Configuration
At startup, a single router instance should be created with a configuration that is the same on any host.
```cs
var config = new RouterConfig();
// The first (default) channel should be ReliableSequenced:
config.AddChannel(0, QosType.ReliableSequenced);

var router = new Router(config);
```

## State Management

### Server
Successfully calling `StartServer` will set the router's state to `Server` immediately.
```cs
// Start a server on port 5500 with a maximum of 100 connections:
if (router.StartServer(5500, 100)) {
	// Server has been started.
}

// Start a server on a port between 5000 and 6000 with a maximum of 100 connections:
int port;
if (router.StartServer(5000, 6000, 100, out port)) {
	// Server has been started.
}
```

### Client
After calling `Connect`, the router's state will be `Client` immediately. If the connection fails, the state will be reset to `None` otherwise the connection to the server will be added to the router's connection set.
```cs
// Connect to 'example.com' on port 5500:
router.Connect('example.com', 5500);

// Connect to some other server on port 5500:
router.Connect(IPAddress.Parse('39.225.249.198'), 5500);
```

### Shutdown
Shutting down a router will result in closing all connections and setting the state to `None` after all connections are closed. During shutdown, no new connections are accepted.
```cs
// Softly shutdown & notify all connected hosts.
router.Shutdown();

// Hard shutdown. All connections are removed immediately without notifying hosts.
router.ShutdownImmediate();
```

## Providing static API
The `Unicorn.GlobalRouterBase` class exposes the same API as the `Router` class does in exception that it's static.
```cs
public class Net : GlobalRouterBase<MyRouterImplementation> {
	[RuntimeInitializeOnLoadMethod]
	private static void Init() {
		// Set the global router instance:
		// If an instance already exists, it's ShutdownImmediate method is called.
		Init(new MyRouterImplementation(config));
	}
}

// Somewhere else:
// Start a server on port 5500 with a maximum of 100 connections:
Net.StartServer(5500, 100);
```

<br/>



# Implementing Networking
When extending the `Unicorn.Router` class, you may want to override the following methods:

## Started()
Called on client &amp; server immediately by calling `Connect` or `StartServer`. Within this method you can start watching the connection set for changes or initialize other stuff like client or server logic.
```cs
// Used to stop handling new connections:
IDisposable connectionAdded;

protected override void Started() {
	connectionAdded = Connections.Added(conn => {
		if (IsServer) {
			// A client connected!
		} else {
			// Connected to the server!
		}
	}, false);
}
```

## ShuttingDown()
Called on client &amp; server when a shutdown has been initiated.

## Stopped()
Called on client &amp; server when a shutdown has completed. During this call the router's state will still be set to `Server` or `Client`.
```cs
protected override void Stopped() {
	// Stop handling new connections (see Started example):
	connectionAdded.Dispose();
}
```

## Network Messages
When dealing with network messages you always have to ensure that a network message is deserialized the same way it was serialized. Otherwise strange errors will occur!

### Receive(Message msg)
Called to handle an incoming network message. The message is an `Unicorn.IO.DataReader` instance which provides useful methods for deserializing the message content.<br/>
The `msg.Sender` property contains the connection that sent the message.
```cs
protected override void Receive(Message msg) {
	var text = msg.ReadString();
	Debug.LogFormat("A remote host says: {0}", text);
}
```

### Sending Messages
Messages can be sent using a connections `Send` method directly:
```cs
// Send a message on channel 0 to a single connection:
var payload = new MemoryWriter();
payload.Write("Hello World!");
conn.Send(0, payload.GetBuffer(), payload.Length);
```

Using the `ConnectionExtensions.Send` method sends a message to an `IEnumerable<Connection>` instance. A single connection is also an `IEnumerable<Connection>`, so you can do:
```cs
conn.Send(0, payload => {
	payload.Write("Hello World!");
});
```
