using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static DataManager _unique;
    int _successCount = 0;
    int _failedCount = 0;
    int _coinCount = 0;

    public static DataManager _instance
    {
        get { return _unique; }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _unique = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
