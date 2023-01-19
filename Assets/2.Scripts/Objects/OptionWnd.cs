using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionWnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseButton()
    {
        gameObject.SetActive(false);
        PlayerController._instance._GamePlay = true;
        EFXPlayManager._instance.PlayEFX("ButtonSelect");
    }
}
