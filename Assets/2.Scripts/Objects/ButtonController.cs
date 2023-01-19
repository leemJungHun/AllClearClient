using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class ButtonController : MonoBehaviour
{
    Material _origin;
    Material _checkOrigin;
    [SerializeField] Material _press;
    [SerializeField] MeshRenderer _checkBox;
    [SerializeField] Slider _startBar;
    MeshRenderer _myMaterial;
    TextMeshPro _myText;
    float _copyTime = 0f;
    bool _isPress = false;
    bool _isCheck = false;
    string _originText = string.Empty;
    // Start is called before the first frame update
    void Start()
    {
        _myMaterial = transform.GetComponent<MeshRenderer>();
        _origin = _myMaterial.material;
        _checkOrigin = _checkBox.material;
        _myText = transform.GetChild(0).GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.name == "GameStartButton")
        {
            if (UserInfos._instance._gameStart)
            {
                if (_startBar.value == 0)
                {
                    EFXPlayManager._instance.PlayEFX("StartGage");
                }
                _startBar.value += Time.deltaTime / 3;
                _myMaterial.material = _press;
                if (_startBar.value >= 1)
                {
                    SceneManager.LoadScene("IngameScene");
                }
            }
            else
            {
                _startBar.value = 0;
                _myMaterial.material = _origin;
            }
        }
        if (_myText.text.Contains("복사 완료"))
        {
            _copyTime += Time.deltaTime;
            if (_copyTime > 0.3f)
            {
                _myText.text = _originText;
                _copyTime = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isPress)
            {
                EFXPlayManager._instance.PlayEFX("SpaceSelect");
                switch (_myText.text)
                {
                    case "게임시작":
                        StartManager._instance.StartGameButton();
                        break;
                    case "환경설정":
                        StartManager._instance.SettingButton();
                        break;
                    case "게임종료":
                        StartManager._instance.ExitButton();
                        break;
                    case "방 생성":
                        StartManager._instance.RoomCreate();
                        break;
                    case "방 입장":
                        StartManager._instance.RoomJoin();
                        break;
                    case "뒤로가기":
                        StartManager._instance.BackButton();
                        break;
                    case "방나가기":
                        NetGameManager._instance.Send_ExitRoom();
                        StartManager._instance.StartGameButton();
                        break;
                    case "체크포인트 설정":
                        _isCheck = !_isCheck;
                        _checkBox.material = _isCheck ? _press : _checkOrigin;
                        break;
                }
                if (_myText.text.Contains("세션ID"))
                {
                    _originText = _myText.text;
                    GUIUtility.systemCopyBuffer = _myText.text.Replace("세션ID:","");
                    _myText.text = "복사 완료";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null&& other.gameObject.CompareTag("Player"))
        {
            if(gameObject.name != "GameStartButton")
            {
                _myMaterial.material = _press;
                EFXPlayManager._instance.PlayEFX("ButtonSelect");
            }
            else
            {
                NetGameManager._instance.Send_GameReady(true);
                Debug.Log("게임 준비");
            }
            _isPress = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Player"))
        {
            if (gameObject.name != "GameStartButton")
            {
                _myMaterial.material = _origin;
            }
            else
            {
                NetGameManager._instance.Send_GameReady(false);

            }
            _isPress = false;
        }
            
    }

}
