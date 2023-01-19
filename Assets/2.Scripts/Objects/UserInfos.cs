using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfos : TSingleton<UserInfos>
{
    long _uuID;
    int _roomIdx;
    string _nick;
    string _sessionID;
    bool _isStart;

    public long _myUUID
    {
        get { return _uuID; }
    }

    public int _myIndex
    {
        get { return _roomIdx; }
    }
    public bool _gameStart
    {
        get { return _isStart; }
    }
    public string _RoomID
    {
        get { return _sessionID; }
    }

    protected override void Init()
    {
        base.Init();
    }

    public void SetUUID(long uuid)
    {
        _uuID = uuid;
    }

    public void SetSessionID(string sessionID)
    {
        _sessionID = sessionID;
    }

    public void SetRoomIdx(int index)
    {
        _roomIdx = index;
    }

    public void SetGameStart(bool isStart)
    {
        _isStart = isStart;
    }
}

