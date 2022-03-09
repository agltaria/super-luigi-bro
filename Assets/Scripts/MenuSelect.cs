using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuSelect : MonoBehaviour
{
    public bool is1P = true;
    public static bool isPlay = false;
    [SerializeField] SpriteRenderer cursor;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void OnMove(InputValue value) {
        if (Mathf.Abs(value.Get<Vector2>().y) > 0.1)
        {
            if (!is1P)
            {
                cursor.transform.localPosition = cursor.transform.localPosition + new Vector3(0f, 16f);
                is1P = true;
            }
            else if (is1P)
            {
                cursor.transform.localPosition = cursor.transform.localPosition + new Vector3(0f, -16f);
                is1P = false;
            }
        }
       // Debug.Log(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        SceneManager.LoadScene(0);
    }
}
