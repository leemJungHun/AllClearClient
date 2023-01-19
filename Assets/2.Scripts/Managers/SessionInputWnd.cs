using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SessionInputWnd : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputText;
    static SessionInputWnd _unique;
    Color _originColor;

    public static SessionInputWnd _instance
    {
        get { return _unique; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _unique = this;
    }
    private void Start() 
    {
        _inputText.Select();
        _originColor = _inputText.placeholder.color;
        _inputText.onValueChanged.AddListener(delegate
        {
            _inputText.placeholder.GetComponent<TMP_Text>().text = "ÀÔ·Â ÈÄ Enter..";
            _inputText.placeholder.color = _originColor;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            PlayerController._instance._GamePlay = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NetGameManager._instance.Send_JoinRoom(_inputText.text);
        }
    }

    public void SetFocusing()
    {
        _inputText.Select();
    }

    public void JoinFailed(string message)
    {
        _inputText.text = string.Empty;
        _inputText.placeholder.GetComponent<TMP_Text>().text = message;
        _inputText.placeholder.color = Color.red;
    }
}
