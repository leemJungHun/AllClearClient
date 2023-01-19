using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFXPlayManager : MonoBehaviour
{
    [SerializeField] AudioClip _buttonSelect;
    [SerializeField] AudioClip _spaceSelect;
    [SerializeField] AudioClip _startGage;
    [SerializeField] AudioClip _stoneAttack;
    [SerializeField] AudioClip _stoneBreak;
    [SerializeField] AudioClip _treeAttack;
    [SerializeField] AudioClip _treeBreak;
    [SerializeField] AudioClip _monsterAttack;
    [SerializeField] AudioClip _monsterDie;
    [SerializeField] AudioClip _playerDie;
    [SerializeField] AudioClip _rootting;

    static EFXPlayManager _unique;
    AudioSource _audioSource;

    public static EFXPlayManager _instance
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
    }

    public void PlayEFX(string name)
    {
        switch (name)
        {
            case "ButtonSelect":
                _audioSource.clip = _buttonSelect;
                break;
            case "SpaceSelect":
                _audioSource.clip = _spaceSelect;
                break;
            case "StartGage":
                _audioSource.clip = _startGage;
                break;
            case "StoneAttack":
                _audioSource.clip = _stoneAttack;
                break;
            case "StoneBreak":
                _audioSource.clip = _stoneBreak;
                break;
            case "TreeAttack":
                _audioSource.clip = _treeAttack;
                break;
            case "TreeBreak":
                _audioSource.clip = _treeBreak;
                break;
            case "MonsterAttack":
                _audioSource.clip = _monsterAttack;
                break;
            case "MonsterBreak":
                _audioSource.clip = _monsterDie;
                break;
            case "PlayerDie":
                _audioSource.clip = _playerDie;
                break;
            case "Rootting":
                _audioSource.clip = _rootting;
                break;
        }
        _audioSource.Play();
    }

    public void StopEFX()
    {
        _audioSource.Stop();
    }
    public void EFXVolum(float value)
    {
        _audioSource.volume = value;
    }
    public float GetVolum()
    {
        return _audioSource.volume;
    }
}
