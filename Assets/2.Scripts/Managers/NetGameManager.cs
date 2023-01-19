using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using DefineServerUtility;
using System.Runtime.InteropServices;

public class NetGameManager : TSingleton<NetGameManager>
{

    const string _ip = "127.0.0.1";
    const short _port = 80;

    Socket _sock;
    Queue<Packet> _sendQ = new Queue<Packet>();
    Queue<Packet> _recvQ = new Queue<Packet>();

    bool _isConnectFaild = false;
    int _retryCount = 3;

    public enum eServerState
    {
        None = 0,
        Connect,
        Room_Wait,
        Room_Enter,
        InGame,
        Failed,
        Upgrade
    }

    public eServerState _serverState
    {
        get; set;
    }


    protected override void Init()
    {
        base.Init();
    }

    public void NetConnect()
    {
        StartCoroutine(Connectings(_ip, _port));
        StartCoroutine(ReceiveProcess());
        StartCoroutine(SendProcess());
    }

    IEnumerator Connectings(string ipAddr, short port)
    {
        int cnt = 0;
        while (true)
        {
            try
            {
                _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _sock.Connect(ipAddr, port);

                break;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
                //서버와 접속이 되지 않습니다....메세지 출력.
                cnt++;
                if (cnt > _retryCount)
                {
                    _isConnectFaild = false;
                    break;
                }
            }
            yield return new WaitForSeconds(3);
        }
    }

    void Update()
    {
        if (_sock != null && _sock.Connected)
        {
            if (_sock.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[ConvertPacketFunc._maxByte];
                try
                {
                    int receiveLength = _sock.Receive(buffer);
                    if (receiveLength != 0)
                    {
                        Packet pack = (Packet)ConvertPacketFunc.ByteArrayToStructure(buffer, typeof(Packet), receiveLength);
                        _recvQ.Enqueue(pack);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
        else
        {
            // 로딩 프로세스
            if (!_isConnectFaild)
            {
                // 메세지창을 띄우고 메세지창을 클릭하면 앱 종료.
                Debug.Log("서버가...");
            }
            else
            {
                // 로딩이 계속 돈다.
            }
        }
    }

    IEnumerator SendProcess()
    {
        while (true)
        {
            if (_sendQ.Count > 0)
            {
                Packet pack = _sendQ.Dequeue();
                byte[] buffer = ConvertPacketFunc.StructureToByteArray(pack);
                _sock.Send(buffer);
            }
            yield return null;
        }
    }

    IEnumerator ReceiveProcess()
    {
        while (true)
        {
            if (_recvQ.Count > 0)
            {
                Packet pack = _recvQ.Dequeue();

                switch ((eReceiveMessage)pack._protocolID)
                {
                    case eReceiveMessage.GivingUUID:
                        Receive_GivingInfo(pack);
                        break;
                    case eReceiveMessage.Room_CreateSuccess:
                        Receive_RoomCreateSuccess(pack);
                        break;
                    case eReceiveMessage.Room_JoinResult:
                        Receive_RoomJoinResult(pack);
                        break;
                    case eReceiveMessage.Player_Position:
                        Receive_PlayerPosition(pack);
                        break;
                    case eReceiveMessage.Player_Move:
                        Receive_PlayerMove(pack);
                        break;
                    case eReceiveMessage.Player_Index:
                        Receive_PlayerIndex(pack);
                        break;
                    case eReceiveMessage.Player_PositionUpdate:
                        Debug.Log("포지션 업데이트 요청");
                        Receive_PositionUpdate();
                        break;
                    case eReceiveMessage.Game_Start:
                        Receive_GameStart(pack);
                        break;
                    case eReceiveMessage.Room_Exit:
                        Receive_ExitRoom(pack);
                        break;
                    case eReceiveMessage.Response_PlayerInfo:
                        Receive_PlayerInfo(pack);
                        break;
                    case eReceiveMessage.Player_Space:
                        Receive_PlayerSpace(pack);
                        break;
                    case eReceiveMessage.Player_Z:
                        Receive_PlayerZ(pack);
                        break;
                }

            }
            yield return null;
        }
    }
    #region [SendProcessingFunc]

    public void Send_CreateRoom()
    {
        Debug.Log("방생성");
        Packet send;
        send._protocolID = (int)eSendMessage.Room_Create;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, 0, null);

        _sendQ.Enqueue(send);
    }

    public void Send_JoinRoom(string sessionID)
    {
        Packet send;
        Send_JoinRoom joinRoom;
        joinRoom._sessionID = sessionID;
        byte[] data = ConvertPacketFunc.StructureToByteArray(joinRoom);
        send._protocolID = (int)eSendMessage.Room_Join;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, Marshal.SizeOf(joinRoom), data);
        _sendQ.Enqueue(send);
    }
    public void Send_ExitRoom()
    {
        Packet send;
        send._protocolID = (int)eSendMessage.Room_Exit;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, 0, null);

        _sendQ.Enqueue(send);
    }

    public void Send_Position(float x, float z, bool init)
    {
        Packet send;
        Send_PlayerPosition position;
        position._sessionID = UserInfos._instance._RoomID;
        position._UUID = UserInfos._instance._myUUID;
        position._x = x;
        position._z = z;
        position._init = init;
        byte[] data = ConvertPacketFunc.StructureToByteArray(position);
        send._protocolID = (int)eSendMessage.Player_Position;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, Marshal.SizeOf(position), data);
        _sendQ.Enqueue(send);
    }
    public void Send_Move(float rx, float mz)
    {
        Packet send;
        Send_PlayerMove move;
        move._sessionID = UserInfos._instance._RoomID;
        move._UUID = UserInfos._instance._myUUID;
        move._rx = rx;
        move._mz = mz;
        byte[] data = ConvertPacketFunc.StructureToByteArray(move);
        send._protocolID = (int)eSendMessage.Player_Move;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, Marshal.SizeOf(move), data);
        _sendQ.Enqueue(send);
    }

    public void Send_GameReady(bool r)
    {
        Packet send;
        Send_GameReady ready;
        ready._sessionID = UserInfos._instance._RoomID;
        ready._index = UserInfos._instance._myIndex;
        ready._ready = r;
        byte[] data = ConvertPacketFunc.StructureToByteArray(ready);
        send._protocolID = (int)eSendMessage.Game_Ready;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, Marshal.SizeOf(ready), data);
        _sendQ.Enqueue(send);
    }

    public void Send_PlayerInfoRequest()
    {
        Packet send;
        send._protocolID = (int)eSendMessage.Request_PlayerInfo;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, 0, null);

        _sendQ.Enqueue(send);
    }

    public void Send_PlayerSpace()
    {
        Packet send;
        send._protocolID = (int)eSendMessage.Player_Space;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, 0, null);

        _sendQ.Enqueue(send);
    }

    public void Send_PlayerZ()
    {
        Packet send;
        send._protocolID = (int)eSendMessage.Player_Z;
        send._targetID = UserInfos._instance._myUUID;
        send = ConvertPacketFunc.CreatePack(send._protocolID, send._targetID, 0, null);

        _sendQ.Enqueue(send);
    }

    #endregion [SendProcessingFunc]


    #region [RecvProcessingFunc]
    void Receive_GivingInfo(Packet recv)
    {
        Receive_GivingInfo myInfo = (Receive_GivingInfo)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_GivingInfo), recv._totalSize);

        UserInfos._instance.SetUUID(myInfo._UUID);
        //UserInfos._instance.BeginGameSet(myInfo._NICK, myInfo._avaraeID);
    }
    void Receive_RoomCreateSuccess(Packet recv)
    {
        Receive_SessionID myInfo = (Receive_SessionID)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_SessionID), recv._totalSize);

        UserInfos._instance.SetSessionID(myInfo._sessionID);
        StartManager._instance.RoomEnter(myInfo._sessionID);
        Debug.Log(myInfo._sessionID);
        //UserInfos._instance.BeginGameSet(myInfo._NICK, myInfo._avaraeID);
    }
    void Receive_RoomJoinResult(Packet recv)
    {
        Receive_JoinResult joinResult = (Receive_JoinResult)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_JoinResult), recv._totalSize);

        if (joinResult._join)
        {
            UserInfos._instance.SetSessionID(joinResult._sessionID);
            StartManager._instance.RoomEnter(joinResult._sessionID);
        }
        else
        {
            SessionInputWnd._instance.JoinFailed(joinResult._sessionID);
        }
        //UserInfos._instance.BeginGameSet(myInfo._NICK, myInfo._avaraeID);
    }

    void Receive_PlayerPosition(Packet recv)
    {
        Receive_PlayerPosition playerPosition = (Receive_PlayerPosition)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerPosition), recv._totalSize);

        PlayerManager._instance.OtherPlayerSet(playerPosition._x, playerPosition._z, playerPosition._UUID);
    }

    void Receive_PlayerMove(Packet recv)
    {
        Receive_PlayerMove move = (Receive_PlayerMove)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerMove), recv._totalSize);

        switch (_serverState)
        {
            case eServerState.Room_Enter:
                PlayerManager._instance.MoveOtherPlayer(move._rx, move._mz, move._UUID);
                break;
            case eServerState.InGame:
                InGameManager._instance.MoveOtherPlayer(move._rx, move._mz, move._UUID);
                break;
            case eServerState.Failed:
                FailedManager._instance.MoveOtherPlayer(move._rx, move._mz, move._UUID);
                break;
            case eServerState.Upgrade:
                UpgradeManager._instance.MoveOtherPlayer(move._rx, move._mz, move._UUID);
                break;
        }
        
    }

    void Receive_PlayerIndex(Packet recv)
    {
        Receive_PlayerIndex index = (Receive_PlayerIndex)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerIndex), recv._totalSize);

        PlayerManager._instance.SetPlayerColor(index._index, index._UUID);
    }

    void Receive_PositionUpdate()
    {
        PlayerController._instance.UpdatePosition();
    }
    
    void Receive_GameStart(Packet recv)
    {
        Receive_GameStart gameStart = (Receive_GameStart)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_GameStart), recv._totalSize);

        UserInfos._instance.SetGameStart(gameStart._start);
    }

    void Receive_ExitRoom(Packet recv)
    {
        Receive_ExitRoom exitRoom = (Receive_ExitRoom)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_ExitRoom), recv._totalSize);

        if (exitRoom._index == 0)
        {
            StartManager._instance.StartGameButton();
        }
        else
        {
            PlayerManager._instance.ExitPlayer(exitRoom._UUID);
        }
        
    }

    void Receive_PlayerInfo(Packet recv)
    {
        Receive_PlayerInfo playerInfo = (Receive_PlayerInfo)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerInfo), recv._totalSize);

        switch (_serverState)
        {
            case eServerState.InGame:
                InGameManager._instance.PlayerSet(playerInfo._index, playerInfo._UUID);
                break;
            case eServerState.Failed:
                FailedManager._instance.PlayerSet(playerInfo._index, playerInfo._UUID);
                break;
            case eServerState.Upgrade:
                UpgradeManager._instance.PlayerSet(playerInfo._index, playerInfo._UUID);
                break;
        }
    }

    void Receive_PlayerSpace(Packet recv)
    {
        Receive_PlayerSpace playerSpace = (Receive_PlayerSpace)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerSpace), recv._totalSize);

        if(_serverState == eServerState.InGame)
        {
            InGameManager._instance.SpaceOtherPlayer(playerSpace._UUID);
        }
        
    }

    void Receive_PlayerZ(Packet recv)
    {
        Receive_PlayerZ playerZ = (Receive_PlayerZ)ConvertPacketFunc.ByteArrayToStructure(recv._datas, typeof(Receive_PlayerZ), recv._totalSize);
        if (_serverState == eServerState.InGame)
        {
            InGameManager._instance.ZOtherPlayer(playerZ._UUID);
        }
    }
    #endregion [RecvProcessingFunc]
}
