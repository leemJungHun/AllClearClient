using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IngredientUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform _target;
    [SerializeField] GameObject _ui;
    [SerializeField] TMP_Text[] _countTxt;
    [SerializeField] int[] _fullCount;
    [SerializeField] int[] _nowCount;
    Color _unFullColor = Color.gray;
    Color _fullColor = Color.white;

    Camera camera;

    bool _isRun = false;

    public bool IsRun
    {
        get { return _isRun; }
    }

    void Start()
    {
        camera = Camera.main;
        _ui.transform.position = _target.position;
        for(int i = 0; i < _countTxt.Length; i++)
        {
            _countTxt[i].text = _nowCount[i] + "/" + _fullCount[i];
        }
    }
    void Update()
    {
        _ui.transform.position = camera.WorldToScreenPoint(_target.position + new Vector3(0, 3f, 0));

        for (int i = 0; i < _countTxt.Length; i++)
        {
            if(_fullCount[i] <= _nowCount[i])
            {
                _countTxt[i].color = _fullColor;
            }
            else
            {
                _countTxt[i].color = _unFullColor;
            }
            if (i==0)
            {
                _isRun = _fullCount[i] <= _nowCount[i] ? true : false;
            }else if (_isRun)
            {
                _isRun = _fullCount[i] <= _nowCount[i] ? true : false;
            }
            
        }
    }

    public bool AddItem(int itemCount, string itemTag)
    {
        for(int i = 0; i< _countTxt.Length; i++)
        {
            if(_countTxt[i].gameObject.tag == itemTag)
            {
                _nowCount[i] += itemCount;
                _countTxt[i].text = _nowCount[i] + "/" + _fullCount[i];
                return true;
            }
        }
        return false;
    }
    public void UseIngredient()
    {
        for (int i = 0; i < _countTxt.Length; i++)
        {
            _nowCount[i] -= _fullCount[i];
            _countTxt[i].text = _nowCount[i] + "/" + _fullCount[i];
        }
    }
}
