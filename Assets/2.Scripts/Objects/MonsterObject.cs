using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterObject : MonoBehaviour
{
    [SerializeField] Transform[] _pos;
    Animator _aniController;
    NavMeshAgent _navAgent;
    
    float _attackDistance = 2f;
    Vector3 _targetPos;
    Transform _playerPos = null;
    int _posCount = 1;
    bool _isDie = false;
    float _destroyTime = 3f;
    float _checkTime = 0f;
    private void Awake()
    {
        
    }

    void Start()
    {
        _aniController = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _targetPos = _pos[_posCount].position;
        _navAgent.destination = _targetPos;
    }

    void Update()
    {
        if (!_isDie)
        {
            if (_playerPos == null)
            {
                if (Vector3.Distance(_targetPos, transform.position) <= 0.1)
                {
                    NextPos();
                    _aniController.SetFloat("ForwardSpeed", 0);
                }
                else
                {
                    _aniController.SetFloat("ForwardSpeed", 0.4f);
                }
            }
            else
            {
                if (Vector3.Distance(_playerPos.position, transform.position) > _attackDistance)
                {
                    _navAgent.isStopped = false;
                    _navAgent.destination = _playerPos.position;
                }
                else
                {
                    _aniController.SetFloat("ForwardSpeed", 1f);
                    _navAgent.isStopped = true;
                }
            }
        }
        else
        {
            _checkTime += Time.deltaTime;
            if (_checkTime >= _destroyTime)
            {
                Destroy(gameObject);
            }
        }
        
    }

    void NextPos()
    {
        if(_posCount == _pos.Length-1)
        {
            _posCount = 0;
        }
        else
        {
            _posCount++;
        }

        _targetPos = _pos[_posCount].position;
        _navAgent.destination = _targetPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject != null)
        {
            if (other.gameObject.CompareTag("Player")){
                _playerPos = other.gameObject.transform;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.CompareTag("Player")){
                _playerPos = null;
            }
        }
    }

    public void MonsterDie()
    {
        _navAgent.isStopped = true;
        _isDie = true;
        transform.localEulerAngles = new Vector3(90, 0, 0);
        _aniController.enabled = false;
    }


}
