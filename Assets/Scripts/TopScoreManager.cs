using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SaveTopScore()
    {
        PlayerPrefs.SetInt("BestScore", ScoreManager.topScore);
        PlayerPrefs.Save();
    }

    public static int LoadTopScore()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            return PlayerPrefs.GetInt("BestScore");
        }
        return 0;
    }
}
