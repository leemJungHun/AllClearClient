using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergeyBarCanvas : MonoBehaviour
{
    public List<Transform> obj;
    public List<GameObject> hp_bar;
    public float _height = 4f;

    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        if(NetGameManager._instance._serverState != NetGameManager.eServerState.InGame)
        {
            gameObject.SetActive(false);
        }
        camera = Camera.main;
        for (int i = 0; i < obj.Count; i++)
        {
            hp_bar[i].transform.position = obj[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < obj.Count; i++)
        {
            hp_bar[i].transform.position = camera.WorldToScreenPoint(obj[i].position + new Vector3(0, _height, 0));
        }
    }
}
