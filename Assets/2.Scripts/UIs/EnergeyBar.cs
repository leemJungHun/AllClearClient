using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergeyBar : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Slider hpbar;
    [SerializeField] float maxHp;
    [SerializeField] float currenthp;
    float _checkTime = 0f;
    float _respownCheckTime = 0f;
    float _respownTime = 5f;
    bool _isDie = false;
    void Update()
    {
        if (_isDie)
        {
            _respownCheckTime += Time.deltaTime;
            if (_respownCheckTime >= _respownTime)
            {
                _isDie = false;
                _respownCheckTime = 0;
            }
        }
        else
        {
            transform.position = target.position + new Vector3(0, 0, 0);
            hpbar.value = currenthp / maxHp;
            _checkTime += Time.deltaTime;

            if (currenthp <= maxHp / 5)
            {
                hpbar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
            }
            else
            {
                hpbar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.green;
            }

            if (_checkTime >= 1f)
            {
                currenthp--;
                _checkTime = 0;
                if (currenthp <= 0)
                {
                    switch (gameObject.tag)
                    {
                        case "Player":
                        case "OtherPlayer":
                            transform.position = transform.parent.position;
                            EFXPlayManager._instance.PlayEFX("PlayerDie");
                            _isDie = true;
                            break;
                        case "Objects":
                            InGameManager._instance.FailedStage();
                            break;
                    }
                }
            }
        }
    }

    public void AddEnergy()
    {
        currenthp += 10;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject != null)
        {
            if (other.gameObject.CompareTag("SafetyZone"))
            {
                if(gameObject.CompareTag("Player")|| gameObject.CompareTag("OtherPlayer"))
                {
                    if (!_isDie)
                    {
                        currenthp = maxHp;
                    }
                    else
                    {
                        currenthp = 1;
                    }
                }
            }
        }
    }
}
