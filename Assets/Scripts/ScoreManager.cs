using System.Collections;
using System.Collections.Generic;
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

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
