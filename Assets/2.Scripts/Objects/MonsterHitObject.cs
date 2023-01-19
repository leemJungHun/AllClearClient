using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitObject : MonoBehaviour
{
    [SerializeField] float _hp = 100;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null)
        {
            if (other.gameObject.CompareTag("Hit"))
            {
                EFXPlayManager._instance.PlayEFX("MonsterAttack");
                _hp -= 20;
                if(_hp <= 0)
                {
                    EFXPlayManager._instance.PlayEFX("MonsterBreak");
                    transform.parent.GetComponent<MonsterObject>().MonsterDie();
                }
            }
        }
    }
}
