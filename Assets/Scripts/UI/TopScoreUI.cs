using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TopScoreUI : MonoBehaviour
{
    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        if (TopScoreManager.LoadTopScore() == 999999)
        {
            text.text = "Top- 000000";
        }
        else
        {
            int topScore = TopScoreManager.LoadTopScore();
            text.text = "Top- " + topScore.ToString("000000");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
