using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public Utilities.GameState gameState = Utilities.GameState.Play;

    private GameObject _pauseScreen;

    [SerializeField] private GameObject _player;
    private void Awake()
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
        _pauseScreen = GameObject.Find("PauseScreen");
        _pauseScreen.SetActive(false);
        
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchPausePlay();
        }
    }

    public void SwitchPausePlay()
    {
        if (gameState == Utilities.GameState.Play)
        {
            gameState = Utilities.GameState.Pause;
            _pauseScreen.SetActive(true);
        }
        else if (gameState == Utilities.GameState.Pause)
        {
            gameState = Utilities.GameState.Play;
            _pauseScreen.SetActive(false);
        }
    }

    public void ReturnToTitle()
    {
        StopAllCoroutines();
        gameState = Utilities.GameState.Title;
        SceneManager.LoadScene("Title");
    }
}
