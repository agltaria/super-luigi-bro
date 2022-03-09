using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int displayScore = 0;
    public UnityEvent scoreChanged = new UnityEvent();
    public static int topScore = 999999;
    [SerializeField] private int _currentScore;

    public int currentScore
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

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UpdateScore());
    }

    // Update is called once per frame
    void Update()
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
        //currentScore += 1; //test
    }

    public void AddScore(int points)
    {
        currentScore += points;
        
    }

    public void ResetScore()
    {
        currentScore = 0;
    }

    private IEnumerator UpdateScore()
    {
        while (true)
        {
            if (displayScore < currentScore)
            {
                displayScore++;
            }
            
            yield return new WaitForSeconds(0.01f); 
        }
    }
}
