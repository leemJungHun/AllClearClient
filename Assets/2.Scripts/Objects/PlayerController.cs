using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [SerializeField] float _runSpeed = 3.5f;
    [SerializeField] float _forwardSpeed = 0.5f;
    [SerializeField] GameObject[] _dropTools;
    [SerializeField] GameObject[] _getTools;
    [SerializeField] Transform _rightHand;
    [SerializeField] GameObject _stonePice;
    [SerializeField] GameObject _woodPice;
    [SerializeField] GameObject _clearItem;
    [SerializeField] Sprite _transIcon;
    [SerializeField] Sprite[] _icons;
    [SerializeField] Sprite[] _cntIcons;

    static PlayerController _unique;
    AudioSource _audioSource;
    Animator _ani;
    CharacterController _charController;
    Camera _camera;
    bool _isMove = false;
    float _mz = 0;
    float _rx = 0;
    float _preMZ = 0;
    float _preRX = 0;
    float _moveTimeLimit = 0f;
    eToolState _toolState;
    eItemState _itemState;
    GameObject _triggerObject;
    GameObject _getObject;
    Vector3 _cameraPos;
    int _itemCnt = 0;

    enum eToolState
    {
        None    = 0,
        Axe,
        Sword,
        Hammer,
    }

    enum eItemState
    {
        None = 0,
        Stone,
        Wood,
        ClearItem
    }

    public static PlayerController _instance
    {
        get { return _unique; }
    }

    public bool _GamePlay
    {
        get; set;
    }

    private void Awake()
    {
        _unique = this;
        _GamePlay = true;
    }

    void Start()
    {
        _ani = transform.GetComponent<Animator>();
        _charController = transform.GetComponent<CharacterController>();
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _cameraPos = _camera.transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (NetGameManager._instance._serverState == NetGameManager.eServerState.InGame)
                {
                    NetGameManager._instance.Send_PlayerSpace();
                }
                if (_triggerObject != null)
                {
                    Debug.Log(_triggerObject.gameObject.tag);
                    switch (_triggerObject.gameObject.tag)
                    {
                        case "Axe_Drop":
                            GetTool(eToolState.Axe, _getTools[0]);
                            break;
                        case "Hammer_Drop":
                            GetTool(eToolState.Hammer, _getTools[1]);
                            break;
                        case "Sword_Drop":
                            GetTool(eToolState.Sword, _getTools[2]);
                            break;
                    }
                }
                else
                {
                    GetTool(eToolState.None, null);
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (NetGameManager._instance._serverState == NetGameManager.eServerState.InGame)
                {
                    NetGameManager._instance.Send_PlayerZ();
                }
                if (_triggerObject != null)
                {
                    Debug.Log(_triggerObject.gameObject.tag);
                    switch (_triggerObject.gameObject.tag)
                    {
                        case "StonePice":
                            GetItem(eItemState.Stone, _icons[0]);
                            break;
                        case "WoodPice":
                            GetItem(eItemState.Wood, _icons[1]);
                            break;
                        case "Objects":
                            if (_itemState == eItemState.ClearItem)
                            {
                                AddClear();
                            }
                            else
                            {
                                AddItem();
                            }
                            break;
                        case "ClearItem":
                            GetItem(eItemState.ClearItem, _icons[2]);
                            break;
                    }
                }
                else
                {
                    GetItem(eItemState.None, _transIcon);
                }
            }

            if (NetGameManager._instance._serverState == NetGameManager.eServerState.InGame)
            {
                _camera.transform.position = _cameraPos + transform.position + transform.parent.transform.position;
            }
            if (NetGameManager._instance._serverState == NetGameManager.eServerState.Room_Enter || NetGameManager._instance._serverState == NetGameManager.eServerState.InGame
                || NetGameManager._instance._serverState == NetGameManager.eServerState.Failed|| NetGameManager._instance._serverState == NetGameManager.eServerState.Upgrade)
            {
                if (_mz != _preMZ || _rx != _preRX)
                {
                    _preMZ = _mz;
                    _preRX = _rx;
                    // 이동 값 전송
                    NetGameManager._instance.Send_Move(_rx, _mz);
                }
            }
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_GamePlay)
        {
            _mz = Input.GetAxisRaw("Vertical");
            _rx = Input.GetAxisRaw("Horizontal");

            if (_mz != 0 || _rx != 0)
            {
                Rotate(_rx, _mz);
                Walk();
                _isMove = true;
                if (!_audioSource.isPlaying)
                    _audioSource.Play();
            }
            else
            {
                _ani.SetFloat("ForwardSpeed", 0);
                _isMove = false;
                _audioSource.Stop();
            }

        }
    }

    void AddItem()
    {
        ObjectsBase objBase = _triggerObject.GetComponent<ObjectsBase>();
        if(objBase != null)
        {
            if(objBase.AddItem(ItemUI._instance.ItemCnt, ItemUI._instance.ItemTag))
            {
                ItemUI._instance.ItemSetting(_transIcon, _transIcon, 0, 0);
                _itemCnt = 0;
                _itemState = eItemState.None;
            }
        }
    }

    void AddClear()
    {
        ObjectsBase objBase = _triggerObject.GetComponent<ObjectsBase>();
        if (objBase != null)
        {
            if (objBase.AddItem(1, ItemUI._instance.ItemTag))
            {
                _itemCnt -= 1;
                if (_itemCnt == 0)
                {
                    ItemUI._instance.ItemSetting(_transIcon, _transIcon, 0, 0);
                    _itemState = eItemState.None;
                }
                else
                {
                    ItemUI._instance.ItemSetting(_icons[2], _cntIcons[_itemCnt - 1], _itemCnt, (int)_itemState);
                }
                
            }
        }
    }

    void GetItem(eItemState itemState, Sprite getItemIcon)
    {
        EFXPlayManager._instance.PlayEFX("Rootting");
        if(itemState == eItemState.None)
        {
            GameObject dropItem = null;
            switch (_itemState)
            {
                case eItemState.Stone:
                    dropItem = Instantiate(_stonePice);
                    break;
                case eItemState.Wood:
                    dropItem = Instantiate(_woodPice);
                    break;
                case eItemState.ClearItem:
                    dropItem = Instantiate(_clearItem);
                    break;
            }
            if (dropItem != null)
            {
                dropItem.transform.position = transform.position;
                dropItem.transform.GetChild(0).GetComponent<CountUI>().SetCount(_itemCnt);
            }
            _itemCnt = 0;
            ItemUI._instance.ItemSetting(getItemIcon, getItemIcon, 0, 0);
        }
        else if(_itemState != itemState)
        {
            GameObject dropItem = null;
            switch (_itemState)
            {
                case eItemState.Stone:
                    dropItem = Instantiate(_stonePice);
                    break;
                case eItemState.Wood:
                    dropItem = Instantiate(_woodPice);
                    break;
                case eItemState.ClearItem:
                    dropItem = Instantiate(_clearItem);
                    break;
            }
            if (dropItem != null)
            {
                dropItem.transform.position = transform.position;
                dropItem.transform.GetChild(0).GetComponent<CountUI>().SetCount(_itemCnt);
            }

            _itemCnt = _triggerObject.transform.GetChild(0).GetComponent<CountUI>().GetCount;
            
            ItemUI._instance.ItemSetting(getItemIcon, _cntIcons[_itemCnt - 1], _itemCnt, (int)itemState);
            Destroy(_triggerObject);
        }
        else
        {
            _itemCnt += _triggerObject.transform.GetChild(0).GetComponent<CountUI>().GetCount;
            if (_itemCnt > 3)
            {
                _triggerObject.transform.GetChild(0).GetComponent<CountUI>().SetCount(_itemCnt - 3);
                _itemCnt = 3;
                ItemUI._instance.ItemSetting(getItemIcon, _cntIcons[_itemCnt - 1], _itemCnt, (int)itemState);
            }
            else
            {
                ItemUI._instance.ItemSetting(getItemIcon, _cntIcons[_itemCnt - 1], _itemCnt, (int)itemState);
                Destroy(_triggerObject);
            }
        }

        _itemState = itemState;

    }

    void GetTool(eToolState toolState, GameObject getTool)
    {
        if (_getObject != null)
        {
            Destroy(_getObject);
        }
        GameObject dropTool = null;
        switch (_toolState)
        {
            case eToolState.Axe:
                dropTool = Instantiate(_dropTools[0]);
                break;
            case eToolState.Hammer:
                dropTool = Instantiate(_dropTools[1]);
                break;
            case eToolState.Sword:
                dropTool = Instantiate(_dropTools[2]);
                break;
        }
        if (dropTool != null)
        {
            dropTool.transform.position = transform.position;
        }
        Destroy(_triggerObject);
        if (getTool != null)
        {
            _getObject = Instantiate(getTool, _rightHand);
        }
        _toolState = toolState;
    }

    public void UpdatePosition()
    {
        NetGameManager._instance.Send_Position(transform.position.x, transform.position.z, false);
    }

    public void InitPosition()
    {
        NetGameManager._instance.Send_Position(transform.position.x, transform.position.z, true);
    }

    public void SetColor(Material material)
    {
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = material;
    }

    void Walk()
    {
        _ani.SetFloat("ForwardSpeed", _forwardSpeed);
        // Rotate() 에서 방향을 바꿔주기 때문에 그 방향대로만 가게 해주면 된다.
        _charController.SimpleMove(transform.forward * _runSpeed);
    }

    void Rotate(float h, float v)
    {
        Vector3 dir = new Vector3(h, 0, v).normalized;
        // 오일러 각을 이용해서 y축으로 회전할 각도를 구해서 회전시켜준다.
        transform.eulerAngles = new Vector3(0, _camera.transform.eulerAngles.y + Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.tag.Contains("Drop")|| other.gameObject.tag.Contains("StonePice")|| other.gameObject.tag.Contains("WoodPice") || other.gameObject.tag.Contains("Objects") || other.gameObject.tag.Contains("ClearItem"))
            {
                Debug.Log(other.gameObject.tag);
                _triggerObject = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.tag.Contains("Drop") || other.gameObject.tag.Contains("StonePice") || other.gameObject.tag.Contains("WoodPice") || other.gameObject.tag.Contains("Objects") || other.gameObject.tag.Contains("ClearItem"))
            {
                _triggerObject = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != null)
        {
            if (!_isMove)
            {
                
                if(other.gameObject.tag == "Stone" && _toolState == eToolState.Hammer 
                    || other.gameObject.tag == "Tree" && _toolState == eToolState.Axe 
                    || other.gameObject.tag == "Monster" && _toolState == eToolState.Sword)
                {
                    _ani.SetFloat("ForwardSpeed", 1);
                }
            }

        }
    }
}
