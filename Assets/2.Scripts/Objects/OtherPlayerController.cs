using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    [SerializeField] GameObject[] _dropTools;
    [SerializeField] GameObject[] _getTools;
    [SerializeField] Transform _rightHand;
    [SerializeField] GameObject _stonePice;
    [SerializeField] GameObject _woodPice;
    [SerializeField] GameObject _clearItem;
    [SerializeField] Sprite _transIcon;
    [SerializeField] Sprite[] _icons;
    [SerializeField] Sprite[] _cntIcons;
    float _runSpeed = 7f;
    float _forwardSpeed = 0.5f;
    Animator _ani;
    CharacterController _charController;
    Camera _camera;
    float _mz = 0;
    float _rx = 0;
    GameObject _triggerObject;
    GameObject _getObject;

    eToolState _toolState;
    eItemState _itemState;
    bool _isMove = false;
    int _itemCnt = 0;

    enum eToolState
    {
        None = 0,
        Axe,
        Sword,
        Hammer,
    }

    enum eItemState
    {
        None = 0,
        StonePice,
        WoodPice,
        ClearItem
    }
    void Start()
    {
        _ani = transform.GetComponent<Animator>();
        _charController = transform.GetComponent<CharacterController>();
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }


    private void FixedUpdate()
    {
        if (_mz != 0 || _rx != 0)
        {
            Rotate(_rx, _mz);
            Walk();
            _isMove = true;
        }
        else
        {
            _ani.SetFloat("ForwardSpeed", 0);
            _isMove = false;
        }
    }

    public void SpaceButton()
    {
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

    public void ZButton()
    {
        if (_triggerObject != null)
        {
            Debug.Log(_triggerObject.gameObject.tag);
            switch (_triggerObject.gameObject.tag)
            {
                case "StonePice":
                    GetItem(eItemState.StonePice, _icons[0]);
                    break;
                case "WoodPice":
                    GetItem(eItemState.WoodPice, _icons[1]);
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
    void AddItem()
    {
        ObjectsBase objBase = _triggerObject.GetComponent<ObjectsBase>();
        if (objBase != null)
        {
            Debug.Log("OtherPlayer itemCnt:" + _itemCnt);
            if (objBase.AddItem(_itemCnt, _itemState.ToString()))
            {
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
            if (objBase.AddItem(1, _itemState.ToString()))
            {
                _itemCnt -= 1;
                if (_itemCnt == 0)
                {
                    _itemState = eItemState.None;
                }

            }
        }
    }

    void GetItem(eItemState itemState, Sprite getItemIcon)
    {
        if (itemState == eItemState.None)
        {
            GameObject dropItem = null;
            switch (_itemState)
            {
                case eItemState.StonePice:
                    dropItem = Instantiate(_stonePice);
                    break;
                case eItemState.WoodPice:
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
        }
        else if (_itemState != itemState)
        {
            GameObject dropItem = null;
            switch (_itemState)
            {
                case eItemState.StonePice:
                    dropItem = Instantiate(_stonePice);
                    break;
                case eItemState.WoodPice:
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

            Destroy(_triggerObject);
        }
        else
        {
            _itemCnt += _triggerObject.transform.GetChild(0).GetComponent<CountUI>().GetCount;
            if (_itemCnt > 3)
            {
                _triggerObject.transform.GetChild(0).GetComponent<CountUI>().SetCount(_itemCnt - 3);
                _itemCnt = 3;
            }
            else
            {
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

    public void PlayerMove(float rx, float mz)
    {
        _rx = rx;
        _mz = mz;
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
            if (other.gameObject.tag.Contains("Drop") || other.gameObject.tag.Contains("StonePice") || other.gameObject.tag.Contains("WoodPice") || other.gameObject.tag.Contains("Objects") || other.gameObject.tag.Contains("ClearItem"))
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

                if (other.gameObject.tag == "Stone" && _toolState == eToolState.Hammer || other.gameObject.tag == "Tree" && _toolState == eToolState.Axe || other.gameObject.tag == "Monster" && _toolState == eToolState.Sword)
                {
                    _ani.SetFloat("ForwardSpeed", 1);
                }
            }

        }
    }
}
