using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayManager : MonoBehaviour
{
    [SerializeField] AudioClip _lobbyBGM;
    [SerializeField] AudioClip _inGameBGM;

    static BGMPlayManager _unique;
    AudioSource _audioSource;

    public static BGMPlayManager _instance
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
        _audioSource = GetComponent<AudioSource>();
        PlayBGM("lobby");
    }

    public void PlayBGM(string name)
    {
        switch (name)
        {
            case "lobby":
                _audioSource.clip = _lobbyBGM;
                break;
            case "inGame":
                _audioSource.clip = _inGameBGM;
                break;
        }
        _audioSource.Play();
    }

    public void BGMVolum(float value)
    {
        _audioSource.volume = value;
    }
    public float GetVolum()
    {
        return _audioSource.volume;
    }
}
