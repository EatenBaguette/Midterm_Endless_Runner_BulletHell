using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameState gameState;

    public AudioBehavior audio;
    void Awake()
    {
        gameState.Title();
    }
    // Update is called once per frame

    void Start()
    {
        audio = AudioBehavior.Instance;
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StopAllCoroutines();
            audio.PressStart();
            SceneManager.LoadScene("Game");
        }
    }
}
