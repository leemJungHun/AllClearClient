using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectObject : MonoBehaviour
{
    [SerializeField] GameObject _carving;
    int _hp = 30;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null)
        {
            if (other.gameObject.CompareTag("Hit"))
            {
                if (gameObject.tag.Contains("Stone")){
                    EFXPlayManager._instance.PlayEFX("StoneAttack");
                }
                else
                {
                    EFXPlayManager._instance.PlayEFX("TreeAttack");
                }
                _hp -= 10;
                if(_hp <= 0)
                {
                    if (gameObject.tag.Contains("Stone"))
                    {
                        EFXPlayManager._instance.PlayEFX("StoneBreak");
                    }
                    else
                    {
                        EFXPlayManager._instance.PlayEFX("TreeBreak");
                    }
                    Instantiate(_carving).transform.position = transform.position;

                    Destroy(gameObject);
                }
            }
        }
    }
}
