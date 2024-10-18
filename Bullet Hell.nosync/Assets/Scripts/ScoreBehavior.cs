using System;
using UnityEngine;
using TMPro;
public class ScoreBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreUI;

    [SerializeField] private float _scoreMultiplier;
    
    private float _oldRawScore;
    private float _newRawScore;
    private float _multipliedScore;

    private int _score;
    
    public float Score
    {
        get => _score;

        set
        {
            _oldRawScore = _newRawScore;
            _multipliedScore += ((value - _oldRawScore)) * _scoreMultiplier;
            _score = Mathf.FloorToInt(_multipliedScore);
            _newRawScore = value;

            _scoreUI.text = _score.ToString();
        }
    }

    private void Start()
    {
        Score = 0;
    }
}