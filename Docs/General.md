# Unicorn

## [Routers](Router.md)
Network connections, messages &amp; the network states are managed by so-called "Routers". The most basic implementation is the `Unicorn.Router` class.

## Connections
A network connection (`Unicorn.Connection`) represents a single connection from the local to a remote host.

## [Connection Sets/Groups](Connections.md)
Unicorn comes with specialized collections for connection management that can be observed or can have sub-sets. These collections can be found in the `Unicorn.Util` namespace.

## Network States
There are three possible states that are defined in the `Unicorn.RouterStates` enum.

#### Server
As a server a router manages multiple connections and accepts every incoming connection request.

#### Client
As a client the router connects to a server once and only acceps a single connection from this server.

#### None
No network is used at all.
