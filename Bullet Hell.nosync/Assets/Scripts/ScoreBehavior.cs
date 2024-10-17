using UnityEngine;
using TMPro;
public class ScoreBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreUI;

    [SerializeField] private float _scoreMultiplier = 1f;

    private int _currentScore = 0;
    private int _lastScore = 0;
    private int _additionalScore;
    private int _score = 0;
    public float Score
    {
        get => _score;

        set
        {
            _lastScore = _currentScore;
            _currentScore = Mathf.FloorToInt(value * _scoreMultiplier);

            if (_lastScore > _currentScore)
            {
                _additionalScore = _lastScore - _currentScore;
            }
            
            _score = _currentScore + _additionalScore;
            
            _scoreUI.text = _score.ToString();
        }
    }

    private void Start()
    {
        Score = 0;
    }
}