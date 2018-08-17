using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public static class GameMsgType
{   
    public static readonly short CheckProto = 1001;
    public static readonly short Snapshot = 1002;
    public static readonly short PlayerInput = 1003;
}


public class SnapshotMessage : MessageBase
{
    public Vector2 BallPos;
    public float RacketAXPos;
    public float RacketBXPos;
    public int ScoreA;
    public int ScoreB;
    public float TS;
}

public class InputMessage : MessageBase
{   
    public float MouseXPos;
    public float TS;
}

public static class NetworkConfiguration
{
    private static HostTopology s_hostTopology;

    public static int ChannelReliableSequenced { get; private set; }
    public static int ChannelUnreliableSequenced { get; private set; }

    public static HostTopology GetHostTopology()
    {
        if (s_hostTopology == null)
        {
            var config = new ConnectionConfig();            
            ChannelReliableSequenced = config.AddChannel(QosType.ReliableSequenced);
            ChannelUnreliableSequenced = config.AddChannel(QosType.UnreliableSequenced);
            s_hostTopology = new HostTopology(config, 2);
        }

        return s_hostTopology;
    }

    static NetworkConfiguration()
    {
        GetHostTopology();
    }
}

public static class PongNetworkManager
{
    public const int PORT = 7070;

    public static bool IsServer { get { return NetworkServer.active; } }
    public static bool IsClient { get { return Client != null; } }

    private static List<NetworkConnection> s_connBuffer = new List<NetworkConnection>();
    public static NetworkClient Client { get; private set; }

    private static readonly EmptyMessage s_emptyMessage = new EmptyMessage();

    public static int StartServer(Action<NetworkConnection> onDisconnect)
    {   
        NetworkServer.RegisterHandler(MsgType.Disconnect, m =>
        {
            if (onDisconnect != null)
                onDisconnect(m.conn);
        });

        NetworkServer.RegisterHandler(GameMsgType.CheckProto, m =>
        {
            Debug.Log("GOT MESSAGE");
        });

        NetworkServer.RegisterHandler(GameMsgType.PlayerInput, m =>
        {
            var msg = m.ReadMessage<InputMessage>();
            //TODO move it to snapshot mgr!
            PPong.Game.PongGame.Instance.ApplyPlayerInput(msg);
        });


        NetworkServer.Configure(NetworkConfiguration.GetHostTopology());

        if (!NetworkServer.Listen(PORT))
        {
            Debug.LogError("NetworkManager : Failed to start server, port is busy");
            return -1;
        }
        else
        {
            Debug.Log("Server started");
        }

        return PORT;
    }


    public static NetworkClient ConnectClient(string address, int port, Action<NetworkConnection> onConnected, Action onDisconnect)
    {
        if (Client != null)
            return Client;        
        Client = new NetworkClient();
        Client.Configure(NetworkConfiguration.GetHostTopology());
        Client.Connect(address, port);        
        Client.RegisterHandler(MsgType.Connect, m => CL_OnConnected(m, onConnected));
        Client.RegisterHandler(MsgType.Disconnect, m => CL_OnDisconnect(m, onDisconnect));

        Client.RegisterHandler(GameMsgType.Snapshot, m =>
        {
            var msg = m.ReadMessage<SnapshotMessage>();
            //TODO move it to snapshot mgr!
            PPong.Game.PongGame.Instance.ApplySnapshot(msg);
        });



        return Client;
    }

    private static void CL_OnDisconnect(NetworkMessage msg, Action onDisconnect)
    {
        Debug.Log("Client: onDisconnect");
        msg.conn.Disconnect();

        if (onDisconnect != null)
            onDisconnect();
    }

    private static void CL_OnConnected(NetworkMessage msg, Action<NetworkConnection> onConnected)
    {
        Debug.Log("Client: onConnected");
        SendToServer(GameMsgType.CheckProto);
        if (onConnected != null)
            onConnected(msg.conn);
    }


    private static bool IsConnected(NetworkConnection conn)
    {
        return conn != null && conn.isConnected && !string.IsNullOrEmpty(conn.address);
    }

    public static void SendMessage(NetworkConnection conn, short msgType, MessageBase msg = null, int channel = -1)
    {
        if (!IsConnected(conn) )
            return;

        if (channel == -1)
            channel = NetworkConfiguration.ChannelReliableSequenced;

        if (msg == null)
            msg = s_emptyMessage;

        conn.SendByChannel(msgType, msg, channel);
    }

    public static void SendToClients(short msgType, MessageBase msg = null, int channel = -1)
    {
        if (!NetworkServer.active)
        {
            Debug.LogError("Can't send a message to clients, server isn't active");
            return;
        }

        var conns = SV_GetConnections();
        for (int i = 0; i < conns.Count; i++)
        {
            SendMessage(conns[i], msgType, msg, channel);
        }
    }

    public static void SendToServer(short msgType, MessageBase msg = null, int channel = -1)
    {
        if (Client == null)
        {
            Debug.LogError("Can't send a message to server, client object doesn't exist");
            return;
        }

        SendMessage(Client.connection, msgType, msg, channel);
    }

    public static ReadOnlyCollection<NetworkConnection> SV_GetConnections()
    {
        s_connBuffer.Clear();

        foreach (NetworkConnection c in NetworkServer.connections)
        {
            if (c == null)
                continue;           

            s_connBuffer.Add(c);
        }

        return s_connBuffer.AsReadOnly();
    }

    public static void CL_Shutdown()
    {
        if (Client == null)
            return;

        Client.Disconnect();
        Client.Shutdown();
        Client = null;
    }

    public static void SV_Shutdown()
    {
        NetworkServer.Shutdown();
    }


}
