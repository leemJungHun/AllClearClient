using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CountUI : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] GameObject _ui;
    [SerializeField] TMP_Text _countTxt;
    int _itemCount = 1;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        _countTxt.text = _itemCount.ToString();
        _ui.transform.position = _target.position;
        if (_itemCount == 1)
        {
            _ui.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _ui.transform.position = camera.WorldToScreenPoint(_target.position + new Vector3(0, 1f, 0));
    }

    public void SetCount(int itemCount)
    {
        _itemCount = itemCount;
        _countTxt.text = _itemCount.ToString();
        if (_itemCount != 1)
        {
            _ui.SetActive(true);
        }
    }

    public int GetCount
    {
        get { return _itemCount; }
    }
}
