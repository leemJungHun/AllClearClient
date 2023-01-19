using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearObject : MonoBehaviour
{
    [SerializeField] Material _runMaterial;
    [SerializeField] Material _stopMaterial;
    [SerializeField] Transform _clearPosition;
    [SerializeField] GameObject _clearItem;
    MeshRenderer _renderer;
    ObjectsBase _myBase;
    float _checkTime = 0f;
    float _addTime = 10f;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = transform.GetComponent<MeshRenderer>();
        _myBase = transform.GetComponent<ObjectsBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_myBase.IsRun)
        {
            _renderer.material = _runMaterial;
            _checkTime += Time.deltaTime;
            if (_checkTime >= _addTime)
            {
                _myBase.UseIngredient();
                Instantiate(_clearItem, _clearPosition);
                _checkTime = 0f;
            }
        }
        else
        {
            _renderer.material = _stopMaterial;
            _checkTime = 0f;
        }
    }
}
