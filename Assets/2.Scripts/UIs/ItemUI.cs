using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] Image _item;
    [SerializeField] Image _itemCount;
    static ItemUI _unique;
    int _itemCnt = 0;
    eItemState _itemState;

    enum eItemState
    {
        None        = 0,
        StonePice,
        WoodPice,
        ClearItem
    }

    public string ItemTag
    {
        get { return _itemState.ToString(); }
    }


    public static ItemUI _instance
    {
        get { return _unique; }
    }

    public int ItemCnt
    {
        get { return _itemCnt; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _unique = this;
    }

    public void ItemSetting(Sprite itemIcon, Sprite cntIcon, int itemCount, int itemState)
    {
        _item.sprite = itemIcon;
        _itemCount.sprite = cntIcon;
        _itemCnt = itemCount;
        _itemState = (eItemState)itemState;
    }
}

