using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfxSoundController : MonoBehaviour
{
    Slider _efxSlider;
    // Start is called before the first frame update
    void Start()
    {
        _efxSlider = GetComponent<Slider>();
        _efxSlider.value = EFXPlayManager._instance.GetVolum();
        _efxSlider.onValueChanged.AddListener(delegate
        {
            EFXPlayManager._instance.EFXVolum(_efxSlider.value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
