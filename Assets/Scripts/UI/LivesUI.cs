using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private GameObject _livesContainer;
    [SerializeField] private GameObject _gameOverContainer;

    private void Start()
    {
        if (PlayerPrefs.GetInt("MarioLives") > 0)
        {
            _livesContainer.SetActive(true);
            _gameOverContainer.SetActive(false);
            _livesText.text = PlayerPrefs.GetInt("MarioLives").ToString();
            StartCoroutine(DeathTimer());
        }
        else
        {
            _livesContainer.SetActive(false);
            _gameOverContainer.SetActive(true);
            StartCoroutine(RestartGameTimer());
        }
     
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("Level 1-1");
        SceneManager.UnloadSceneAsync("LevelStartScreen");
    }
    
    IEnumerator RestartGameTimer()
    {
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene("TitleScreen");
        SceneManager.UnloadSceneAsync("LevelStartScreen");
    }
    
}
