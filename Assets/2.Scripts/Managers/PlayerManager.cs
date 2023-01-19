using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject _other;
    [SerializeField] Material[] _colors;
    [SerializeField] Material _origin;
    Dictionary<long, GameObject> _otherDic;
    // Start is called before the first frame update
    static PlayerManager _unique;
    public static PlayerManager _instance
    {
        get { return _unique; }
    }

    private void Awake()
    {
        _unique = this;
    }
    private void Start()
    {
        _otherDic = new Dictionary<long, GameObject>();
    }

    public void OtherPlayerSet(float x, float z, long uuid)
    {
        GameObject other = Instantiate(_other, transform);
        other.name = uuid.ToString();
        other.transform.position = new Vector3(x, 0, z);
        _otherDic.Add(uuid, other);
       
    }

    public void SetPlayerColor(int index, long uuid)
    {
        if(uuid == UserInfos._instance._myUUID)
        {
            PlayerController._instance.SetColor(_colors[index]);
            UserInfos._instance.SetRoomIdx(index);
        }
        else
        {
            _otherDic[uuid].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = _colors[index];
        }
    }

    public void SetPlayerOriginColor()
    {
        PlayerController._instance.SetColor(_origin);
    }

    public void ExitPlayer(long uuid)
    {
        Destroy(_otherDic[uuid]);
        _otherDic.Remove(uuid);
    }

    public void ExitRoom()
    {
        foreach (KeyValuePair<long, GameObject> other in _otherDic)
        {
            Destroy(other.Value);
        }
        _otherDic.Clear();
    }

    public void MoveOtherPlayer(float rx, float mz, long uuid)
    {
        if (_otherDic.ContainsKey(uuid))
        {
            _otherDic[uuid].GetComponent<OtherPlayerController>().PlayerMove(rx, mz);
        }
    }

}
