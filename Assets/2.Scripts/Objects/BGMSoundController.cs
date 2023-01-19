using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMSoundController : MonoBehaviour
{
    Slider _bgmSlider;
    // Start is called before the first frame update
    void Start()
    {
        _bgmSlider = GetComponent<Slider>();
        _bgmSlider.value = BGMPlayManager._instance.GetVolum();
        _bgmSlider.onValueChanged.AddListener(delegate
        {
            BGMPlayManager._instance.BGMVolum(_bgmSlider.value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
