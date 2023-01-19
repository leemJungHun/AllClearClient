using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsBase : MonoBehaviour
{
    GameObject _childCanvas;
    IngredientUI _childIngredientUI;
    // Start is called before the first frame update

    public bool IsRun
    {
        get { return _childIngredientUI.IsRun; }
    }

    void Start()
    {
        _childCanvas = transform.GetChild(0).gameObject;
        _childIngredientUI = _childCanvas.GetComponent<IngredientUI>();
        _childCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddItem(int itemCount, string itemTag)
    {
        return _childIngredientUI.AddItem(itemCount, itemTag);
    }

    public void UseIngredient()
    {
        _childIngredientUI.UseIngredient();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null)
        {
            if (other.gameObject.tag == "Player")
            {
                _childCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.tag == "Player")
            {
                _childCanvas.SetActive(false);
            }
        }
    }
}
