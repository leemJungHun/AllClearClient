using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [SerializeField] Transform[] _initPositions;
    [SerializeField] GameObject _other;
    [SerializeField] GameObject _player;
    [SerializeField] Material[] _colors;
    [SerializeField] Material _origin;
    [SerializeField] string[] _quests;
    [SerializeField] TMP_Text _questText;
    Dictionary<long, GameObject> _otherDic;
    static InGameManager _unique;
    int _questNum = 0;
    int _clearCount = 0;

    public static InGameManager _instance
    {
        get { return _unique; }
    }
    void Awake()
    {
        _unique = this;
        NetGameManager._instance._serverState = NetGameManager.eServerState.InGame;
        NetGameManager._instance.Send_PlayerInfoRequest();
        BGMPlayManager._instance.PlayBGM("inGame");
        _otherDic = new Dictionary<long, GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //_questNum = Random.Range(0, _quests.Length);
        _questText.text = _quests[_questNum].Replace("\\n", "\n");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearObject()
    {
        _clearCount++;
        if (_clearCount == 3)
        {
            UpgradeStage();
        }
    }

    public void FailedStage()
    {
        NetGameManager._instance._serverState = NetGameManager.eServerState.Failed;
        SceneManager.LoadScene("FailedScene");
    }

    public void UpgradeStage()
    {
        NetGameManager._instance._serverState = NetGameManager.eServerState.Upgrade;
        SceneManager.LoadScene("UpgradeScene");
    }
    public void PlayerSet(int index, long uuid)
    {
        if (uuid == UserInfos._instance._myUUID)
        {
            GameObject player = Instantiate(_player, _initPositions[index]);
            player.name = "Player";
            player.transform.position = _initPositions[index].position;
            player.transform.eulerAngles = new Vector3(0, -219.363f, 0);

            PlayerController._instance.SetColor(_colors[index]);
            UserInfos._instance.SetRoomIdx(index);
        }
        else
        {
            GameObject other = Instantiate(_other, _initPositions[index]);
            other.name = uuid.ToString();
            other.transform.position = _initPositions[index].position;
            other.transform.eulerAngles = new Vector3(0, -219.363f, 0);
            _otherDic.Add(uuid, other);
            _otherDic[uuid].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = _colors[index];
        }
    }

    public void MoveOtherPlayer(float rx, float mz, long uuid)
    {
        if (_otherDic.ContainsKey(uuid))
        {
            _otherDic[uuid].GetComponent<OtherPlayerController>().PlayerMove(rx, mz);
        }
    }

    public void SpaceOtherPlayer(long uuid)
    {
        if (_otherDic.ContainsKey(uuid))
        {
            _otherDic[uuid].GetComponent<OtherPlayerController>().SpaceButton();
        }
    }

    public void ZOtherPlayer(long uuid)
    {
        if (_otherDic.ContainsKey(uuid))
        {
            _otherDic[uuid].GetComponent<OtherPlayerController>().ZButton();
        }
    }
}
