using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Debug.LogWarning($"More than one {nameof(UIManager)}.");
            DestroyImmediate(this.gameObject);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
