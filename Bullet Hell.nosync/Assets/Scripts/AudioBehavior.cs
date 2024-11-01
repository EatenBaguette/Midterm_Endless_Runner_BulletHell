using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioBehavior : MonoBehaviour
{
    public Utilities.GameState gameState;
    private Utilities.GameState _previousGameState;
    
    public static AudioBehavior Instance;
    
    [SerializeField] private AudioClip _gameOver;
    

    public float bpm = 140.0f;
    public int numBeatsPerSegment = 160;
    public AudioClip[] clips = new AudioClip[6];

    private double nextEventTime;
    private int flip = 0;
    private AudioSource[] audioSources = new AudioSource[6];
    private bool running = false;
    
    [SerializeField] private float fadeDuration = 1.5f;
    float startValue=0;
    float endValue=1;
    
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(obj: this);
        }

        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        gameState = Utilities.GameState.Title;
        
        _previousGameState = gameState;
        
        for (int i = 0; i < 6; i++)
        { 
            GameObject child = new GameObject("Player"); child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
        }

        nextEventTime = AudioSettings.dspTime + 2.0f;
        running = true;
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        double time = AudioSettings.dspTime;

        if (time + 1.0f > nextEventTime)
        {
            // We are now approx. 1 second before the time at which the sound should play,
            // so we will schedule it now in order for the system to have enough time
            // to prepare the playback at the specified time. This may involve opening
            // buffering a streamed file and should therefore take any worst-case delay into account.
            for (int i = 0; i < 6; i += 2)
            {
                audioSources[flip + i].clip = clips[flip + i];
                audioSources[flip + i].PlayScheduled(nextEventTime);
            }

            Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime);

            // Place the next event 160 beats from here at a rate of 140 beats per minute
            nextEventTime += 60.0f / bpm * numBeatsPerSegment;

            // Flip between two audio sources so that the loading process of one does not interfere with the one that's playing out
            flip = 1 - flip;
        }

        if (gameState == Utilities.GameState.Title)
        {
            if (_previousGameState != gameState)
            {
                StartCoroutine(FadeToTitle());
            }
            else if (_previousGameState == Utilities.GameState.Title)
            {
                for (int i = 2; i < 6; i++)
                {
                    audioSources[i].volume = startValue;
                }
            }
        }

        if (gameState == Utilities.GameState.Play)
        {
            if (_previousGameState == Utilities.GameState.Title)
            {
                StartCoroutine(FadeToGame());
            }
        }
        
        _previousGameState = gameState;
    }

    private IEnumerator FadeToTitle()
    {
        float timeElapsed = 0;
        
        for (int i = 2; i < 6; i++)
        {
            while (timeElapsed < fadeDuration)
            {
                audioSources[i].volume = Mathf.Lerp(endValue, startValue, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                
                yield return null;
            }
            
            audioSources[i].volume = startValue;
        }
    }
    private IEnumerator FadeToGame()
    {
        float timeElapsed = 0;
        
        for (int i = 2; i < 6; i++)
        {
            while (timeElapsed < fadeDuration)
            {
                audioSources[i].volume = Mathf.Lerp(startValue, endValue, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                
                yield return null;
            }

            audioSources[i].volume = endValue;
        }
    }
}

