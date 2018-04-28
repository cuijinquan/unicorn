# Connection Sets/Groups
Managing and tracking connections can be very complex and can quickly lead to a variety of different bugs. To prevent this, Unicorn provides well thought-out collections designed to manage and respond to changes in super and subsets of connections. They can be found in the `Unicorn.Util` namespace.

<br/>



# Implementations

## Set
This is the most toplevel set of connections that is managed by the `Unicorn.Router` class itself.

## SubSet
The `SubSet` class ensures, that it only contains connections that are in it's superset.
```cs
// Create a sub set with two connections:
var subSet = new SubSet<Connection>(router.Connections);
subSet.Add(connection1);
subSet.Add(connection2);

// When connection1 or connection2 are removed from router.Connections
// they are immediately removed from the sub set.
```

## SetProxy
The `SetProxy` class pretends to be another set of connections as it exposes the same connections and events as it's target set. When the target is changed, the right events for changes are fired.
```cs
var a = new SubSet<Connection>(router.Connections);
a.Add(conn1);
a.Add(conn2);

var b = new SubSet<Connection>(router.Connections);
b.Add(conn2);
b.Add(conn3);

// Create a set that pretends to be the set 'a':
var proxy = new SetProxy<Connection>(a);
// proxy has 'conn1' and 'conn2'.

// Change the target set to 'b':
proxy.Target = b;
// proxy has 'conn2' and 'conn3'.

// Make the proxy empty:
proxy.Target = null;
// proxy no has no connections.
```

## IntersectionSet
The `IntersectionSet` class represents an intersection of two other sets.
An intersection set only contains items that are in both specified sets.
```cs
var a = new SubSet<Connection>(router.Connections);
a.Add(conn1);
a.Add(conn2);

var b = new SubSet<Connection>(router.Connections);
b.Add(conn2);
b.Add(conn3);

// Create a set that intersects a and b.
var i = new IntersectionSet<Connection>(a, b);
// i has 'conn2', but not 'conn1' or 'conn3'.
```

<br/>



# Observable Sets
Sets that implement `IObservableSet` and `IReadonlyObservableSet` provide methods to watch the sets for changes.
```cs
var example = new SubSet<Connection>(router.Connections);

example.Added(conn => {
	// conn has been added to 'example'.
});

example.Removed(conn => {
	// conn has been removed from 'example'.
});
```
### Removing event handlers
`.Added` and `.Removed` functions return an `IDisposable` that removes the event handler when it is disposed.
```cs
var e = example.Added(...);

e.Dispose();
```
### Weak event handlers
Weakly referenced event handlers are useful if you want to remove event handlers automatically when a part of the program is garbage collected.
```cs
public class SomeExample {
	public SomeExample(IObservableSet<Connection> connections) {
		_connected = connections.AddedWeak(Connected);
		_disconnected = connections.RemovedWeak(Disconnected);
	}

	// You should keep track of registered events,
	// so that they are not accidentally removed:
	private IDisposable _connected;
	private IDisposable _disconnected;

	private void Connected(Connection conn) {
		// conn has been added to 'connections'.
	}

	private void Disconnected(Connection conn) {
		// conn has been removed from 'connections'.
	}
}
```
If you are just assigning `_connected` and `_disconnected` fields, you usually want to ignore warnings:
```cs
# pragma warning disable 0414
private IDisposable _connected;
private IDisposable _disconnected;
# pragma warning restore 0414
```
