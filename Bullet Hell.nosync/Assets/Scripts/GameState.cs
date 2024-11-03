using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [SerializeField] private Utilities.GameState _gameState;

    public Utilities.GameState gameState
    {
        get => _gameState;
    }

    private void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Title() {_gameState = Utilities.GameState.Title;}
    public void Play() {_gameState = Utilities.GameState.Play;}
    public void Pause() {_gameState = Utilities.GameState.Pause;}
    public void GameOver() {_gameState = Utilities.GameState.GameOver;}
}
