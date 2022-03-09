using UnityEngine.Events;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;

    public UnityEvent scoreChanged = new UnityEvent();

    private float _currentScore;

    public float currentScore
    {
        get => _currentScore;
        private set
        {
            _currentScore = value;
            scoreChanged.Invoke();
        }
    }
    
    public static ScoreManager Instance;
        
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"More than one {nameof(ScoreManager)}.");
            enabled = false;
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
