using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _livesText;

    private void Start()
    {
        _livesText.text = PlayerPrefs.GetInt("MarioLives").ToString();
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("Level 1-1");
        SceneManager.UnloadSceneAsync("LevelStartScreen");
    }
    
}
