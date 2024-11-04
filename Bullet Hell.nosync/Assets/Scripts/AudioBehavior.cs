using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioBehavior : MonoBehaviour
{
    public static AudioBehavior Instance;

    public GameState gameState;
    private Utilities.GameState _previousGameState;

    [SerializeField] private ArrowBehavior _player;
    
    [SerializeField] float bpm = 140.0f;
    [SerializeField] int numBeatsPerSegment = 160;

    [SerializeField] private AudioClip[] _layers = new AudioClip[3];
    [SerializeField] private AudioClip _gameOver;
    [SerializeField] private AudioClip _pressStart;

    private AudioSource _audioSource;

    [SerializeField] private AudioMixerGroup[] _audioMixerGroups = new AudioMixerGroup[3];
    [SerializeField] private AudioMixerGroup _SFXMixer;

    private bool _layer3IsPlaying = false;
    private bool _isFirstPlay = true;

    private AudioClip[] _clips;

    private double _nextEventTime;
    private int _flip = 0;
    private AudioSource[] _audioSources = new AudioSource[6];
    private bool _running = false;
    
    [SerializeField] private float _fadeDuration = 1.5f;
    private float _minVolume = -80.0f;
    private float _maxVolume = 0.00f;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        gameState = GameState.Instance;
        _previousGameState = gameState.gameState;
        
        _audioSource = GetComponent<AudioSource>();

        _clips = new AudioClip[_layers.Length * 2];

        for (int i = 0; i < _layers.Length * 2; i++)
        {
            GameObject child = new GameObject("Audio Player"); child.transform.parent = gameObject.transform;
            _audioSources[i] = child.AddComponent<AudioSource>();
        }
        
        for (int i = 0; i < _layers.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                _clips[2 * i + j] = _layers[i];

                _audioSources[2 * i + j].outputAudioMixerGroup = _audioMixerGroups[i];
            }
        }
        _audioMixerGroups[0].audioMixer.SetFloat(("volume0"), _maxVolume);
        _audioMixerGroups[1].audioMixer.SetFloat(("volume1"), _minVolume);
        _audioMixerGroups[2].audioMixer.SetFloat(("volume2"), _minVolume);
        
        _nextEventTime = AudioSettings.dspTime + 2.0f;
        _running = true;
    }

    void Update()
    {
        if (!_running)
        {
            return;
        }

        double time = AudioSettings.dspTime;

        if (time + 1.0f > _nextEventTime)
        {
            for (int i = 0; i < 6; i += 2)
            {
                _audioSources[_flip + i].clip = _clips[_flip + i];
                _audioSources[_flip + i].PlayScheduled(_nextEventTime);
            }
            
            _nextEventTime += 60.0f / bpm * numBeatsPerSegment;
            
            _flip = 1 - _flip;
        }

        switch (gameState.gameState)
        {
            //case Utilities.GameState.Title:
               // if (_previousGameState != Utilities.GameState.Title)
                //{
                 //   for (int i = 0; i < 3; i++) {StartCoroutine(Fade(_audioMixerGroups[i], i, _minVolume, 0.15f)); }
                 //   for (int i = 0; i < 6; i++) { _audioSources[i].Stop(); }
                  //  _nextEventTime = AudioSettings.dspTime + 1.5f;
                  //  _audioMixerGroups[0].audioMixer.SetFloat("volume0", _maxVolume);
               // }
              //  break;
            case Utilities.GameState.Play:
                if (_isFirstPlay)
                {
                    _player = GameObject.Find("Player").GetComponent<ArrowBehavior>();
                    _isFirstPlay = false;
                    _layer3IsPlaying = false;
                }
                
                if (_player.Speed > 250f && !_layer3IsPlaying)
                {
                    _layer3IsPlaying = true;
                    
                    StartCoroutine(Fade(_audioMixerGroups[2], 2, _maxVolume, 0.05f));
                    
                }
                else if (_player.Speed < 250f && _layer3IsPlaying)
                {
                    _layer3IsPlaying = false;
                    StartCoroutine(Fade(_audioMixerGroups[2], 2, -10f, _fadeDuration));
                }
                
                switch (_previousGameState)
                {
                    case Utilities.GameState.Title:
                        StartCoroutine(Fade(_audioMixerGroups[1], 1, _maxVolume, _fadeDuration));
                        _layer3IsPlaying = false;
                        break;
                    
                    case Utilities.GameState.GameOver:
                        Debug.Log("Game Over to Play");
                        StopAllCoroutines();
                        StartCoroutine(Fade(_audioMixerGroups[0], 0, _maxVolume, _fadeDuration));
                        StartCoroutine(Fade(_audioMixerGroups[1], 1, _maxVolume, _fadeDuration));
                        _layer3IsPlaying = false;
                        break;
                        
                    case Utilities.GameState.Pause:
                        StartCoroutine(Fade(_audioMixerGroups[1], 1,  _maxVolume, 0.15f));
                        if (_layer3IsPlaying) { StartCoroutine(Fade(_audioMixerGroups[2], 2, _maxVolume, 0.15f)); }
                        break;
                }
                break;
            
            case Utilities.GameState.Pause:
                if (_previousGameState != gameState.gameState)
                {
                    StartCoroutine(Fade(_audioMixerGroups[2], 2, _minVolume, 0.15f));
                    StartCoroutine(Fade(_audioMixerGroups[1], 1, _minVolume, 0.15f));
                }
                break;
            
            case Utilities.GameState.GameOver:
                if (_previousGameState != gameState.gameState)
                {
                    _isFirstPlay = true;
                    _layer3IsPlaying = false;
                    StartCoroutine(Fade(_audioMixerGroups[2], 2, _minVolume, 0.15f));
                    StartCoroutine(Fade(_audioMixerGroups[1], 1, _minVolume, 0.15f));
                    StartCoroutine(Fade(_audioMixerGroups[0], 0, _minVolume, 0.15f));
                    
                    _audioSource.outputAudioMixerGroup = _SFXMixer;
                    _audioSource.PlayOneShot(_gameOver);
                }
                break;
        }
        
        _previousGameState = gameState.gameState;
    }
    
    private IEnumerator Fade(AudioMixerGroup mixerGroup, int groupNumberInArray, float targetVolume, float duration)
    {
        double elapsedTime = 0;
        double oldTime = AudioSettings.dspTime;

        float startVolume;
        
        mixerGroup.audioMixer.GetFloat(("volume" + groupNumberInArray), out startVolume);

        while (elapsedTime < duration)
        {
            mixerGroup.audioMixer.SetFloat(("volume" + groupNumberInArray),
                Mathf.Lerp(startVolume, targetVolume, (float)elapsedTime / duration));
            
            elapsedTime += AudioSettings.dspTime - oldTime;
            oldTime = AudioSettings.dspTime;

            yield return null;
        }
        
        mixerGroup.audioMixer.SetFloat(("volume" + groupNumberInArray), targetVolume);
    }

    public void PressStart()
    {
        _audioSource.outputAudioMixerGroup = _SFXMixer;
        _audioSource.PlayOneShot(_pressStart);
    }
}

