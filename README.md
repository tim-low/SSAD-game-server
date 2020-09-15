# SUPERSAD Game-Server

## CZ3003 Software System Analysis & Design

**__SUPERSAD Game-Server__** is coded in C# using **System.Net.Sockets**.
It act as a authoritative server & communication middle-man for [**SE Party**](https://gitlab.com/super-sad/game-client), broadcasting
packets from a client to multiple clients as long as they are connected and related in the game. It also act as a bridge between the
client and the underlying database. It also contain multiple state for user of the client such as Lobby/Room/Game as user are only
allowed certain action at certain stage of the game.

[**Packets**](https://gitlab.com/super-sad/game-server/blob/master/Game-Server/Network/Packet.cs) are transmitted from one end of the protocol to another end.
Packets received by the server are prefixed with a Cmd indicating that it is a command sent from a client, while packets sent by the server are postfixed 
with Ack to indicate that an acknowledgement from the server is received and a response is given back to the client. In order to streamline the process of
creating packets that both the front-end client and back-end server development team uses, we maintain a shared logs of packets opscode and packets structures.
Packets are generally bytes of data that are packed in a way that the receiving ends know how to process and what to read it for.

## Features
- Multi-threaded Server
- Reliable Data Transmission (TCP)
- Symmetric Key Encryption
- Cross-platform
- Easy-to-use

## Requirement
- A port forwarded server with static IP addressing is required for connectivity from clients
- .NET Core SDK & Runtime is required

## Contribution
- **Jun Ao**: Game Component, Networking, Database EFCore Implementation
- **Callista**: Loot Box, Lobby + Room Component, Lobby + Room Integration, Load Testing
- **Kailing**: Database Design, Question Api Component, Web Server Integration, Web Server Testing
- **Sherna**: Database Design, Session Api Component, Web Server Integration, Web Server Testing