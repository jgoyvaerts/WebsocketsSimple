﻿using System.Threading.Tasks;
using PHS.Networking.Models;
using PHS.Networking.Services;
using WebsocketsSimple.Server.Events.Args;
using WebsocketsSimple.Server.Managers;
using WebsocketsSimple.Server.Models;

namespace WebsocketsSimple.Server
{
    public interface IWebsocketServer : ICoreNetworking<WSConnectionServerEventArgs, WSMessageServerEventArgs, WSErrorServerEventArgs>
    {
        Task<bool> SendToConnectionAsync<S>(S packet, IConnectionServer connection) where S : IPacket;
        Task<bool> SendToConnectionAsync(string message, IConnectionServer connection);
        Task<bool> SendToConnectionRawAsync(string message, IConnectionServer connection);
        Task DisconnectConnectionAsync(IConnectionServer connection);

        Task StartReceivingAsync(IConnectionServer connection);
        
        IConnectionServer[] Connections { get; }
        WebsocketConnectionManager ConnectionManager { get; }
    }
}