using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinIconAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] _sprites;

    private int index;

    private void Start()
    {
        StartCoroutine(CoinAnim());

    }

    private void LateUpdate()
    {
       
    }

    IEnumerator CoinAnim()
    {
        
        if (index < _sprites.Length)
        {
            image.sprite = _sprites[index];
            index++;
        }
        else
        {
            index = 0;
        }

        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(CoinAnim());
    }
}
