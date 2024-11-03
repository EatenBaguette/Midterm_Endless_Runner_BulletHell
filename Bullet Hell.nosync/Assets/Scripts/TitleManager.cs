using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameState gameState;
    void Awake()
    {
        gameState.Title();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Game");
        }
    }
}
