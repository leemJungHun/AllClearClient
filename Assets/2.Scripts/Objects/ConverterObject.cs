using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConverterObject : MonoBehaviour
{
    [SerializeField] Material _runConverterMaterial;
    [SerializeField] Material _stopConverterMaterial;
    [SerializeField] EnergeyBar _energy;
    MeshRenderer _renderer;
    ObjectsBase _myBase;
    float _checkTime = 0f;
    float _addTime = 3f;
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
            _renderer.material = _runConverterMaterial;
            _checkTime += Time.deltaTime;
            if (_checkTime >= _addTime)
            {
                _energy.AddEnergy();
                _myBase.UseIngredient();
                _checkTime = 0f;
            }
        }
        else
        {
            _renderer.material = _stopConverterMaterial;
            _checkTime = 0f;
        }
    }


}
