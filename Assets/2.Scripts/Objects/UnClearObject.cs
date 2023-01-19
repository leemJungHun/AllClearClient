using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnClearObject : MonoBehaviour
{
    [SerializeField] Material _clearMaterial;
    [SerializeField] GameObject _safetyZone;
    MeshRenderer _renderer;
    ObjectsBase _myBase;
    bool _isClear = false;
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
            _renderer.material = _clearMaterial;
            _safetyZone.SetActive(true);
            if (!_isClear)
            {
                InGameManager._instance.ClearObject();
                _isClear = true;
            }
        }
    }
}
