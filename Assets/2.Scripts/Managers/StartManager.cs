using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartManager : MonoBehaviour
{
    [SerializeField] TextMeshPro[] _text;
    [SerializeField] GameObject[] _buttons;
    [SerializeField] GameObject _inputWnd;
    [SerializeField] GameObject _optionWnd;
    float _loaddingTime = 0.3f;
    string _loaddingStr = string.Empty;
   
    static StartManager _unique;
    public static StartManager _instance
    {
        get { return _unique; }
    }

    private void Awake()
    {
        _unique = this;
        NetGameManager._instance.NetConnect();
        NetGameManager._instance._serverState = NetGameManager.eServerState.Connect;
    }

    private void Start()
    {
        //Screen.SetResolution(480, 240, false);
        Screen.SetResolution(640, 360, false);
        _buttons[3].SetActive(false);
        _buttons[4].SetActive(false);
    }

    private void Update()
    {
       
        switch (NetGameManager._instance._serverState)
        {
            case NetGameManager.eServerState.Room_Wait:
                _buttons[1].SetActive(false);
                _loaddingTime += Time.deltaTime;
                
                if (_loaddingTime > 0.3f)
                {
                    if(_loaddingStr.Length > 2)
                    {
                        _loaddingStr = string.Empty;
                    }
                    else
                    {
                        _loaddingStr += ".";
                    }
                    Debug.Log(_loaddingStr);
                    _loaddingTime = 0;
                    _text[0].text = "입장중" + _loaddingStr;
                }
                
                break;
        }
    }

    public void StartGameButton()
    {
        NetGameManager._instance._serverState = NetGameManager.eServerState.Connect;
        PlayerManager._instance.ExitRoom();
        PlayerManager._instance.SetPlayerOriginColor();
        _buttons[1].SetActive(true);
        _buttons[3].SetActive(false);
        _buttons[4].SetActive(false);
        _text[0].text = "방 생성";
        _text[1].text = "방 입장";
        _text[2].text = "뒤로가기";
    }

    public void RoomCreate()
    {
        NetGameManager._instance._serverState = NetGameManager.eServerState.Room_Wait;
        NetGameManager._instance.Send_CreateRoom();
    }

    public void RoomEnter(string sessionID)
    {
        _inputWnd.SetActive(false);
        PlayerController._instance.InitPosition();
        PlayerController._instance._GamePlay = true;
        NetGameManager._instance._serverState = NetGameManager.eServerState.Room_Enter;
        _buttons[1].SetActive(true);
        _buttons[3].SetActive(true);
        _buttons[4].SetActive(true);
        _text[0].text = "세션ID:" + sessionID;
        _text[1].text = "체크포인트 설정";
        _text[2].text = "방나가기";
    }

    public void SettingButton()
    {

        _optionWnd.SetActive(true);
        PlayerController._instance._GamePlay = false;
    }

    public void RoomJoin()
    {
        _inputWnd.SetActive(true);
        SessionInputWnd._instance.SetFocusing();
        PlayerController._instance._GamePlay = false;
    }
    
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }

    public void CheckPointSet()
    {

    }

    public void BackButton()
    {
        NetGameManager._instance._serverState = NetGameManager.eServerState.Connect;
        _buttons[1].SetActive(true);
        _buttons[3].SetActive(false);
        _buttons[4].SetActive(false);
        _text[0].text = "게임시작";
        _text[1].text = "환경설정";
        _text[2].text = "게임종료";
    }

}
