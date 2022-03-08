using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Text text;

    private void Awake()
    {
        scoreManager.scoreChanged.AddListener(OnScoreChanged);

        //text = GetComponent<TextMeshProUGUI>();
        //text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnScoreChanged()
    {
        text.text = string.Format("Mario" + "\n" + scoreManager.currentScore.ToString("000000"));//timer.CurrentTime.ToString("000");
    }
}
