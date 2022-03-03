using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlatformer2DTrigger : MonoBehaviour
{
    enum Direction { Down, Left, Right };
    [SerializeField] Direction direction;
    PCPlatformer2D controller;
    int colliders;

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.GetComponent<PCPlatformer2D>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            colliders++;
            if (colliders == 1)
            {
                if (direction.ToString() == "Down") controller.grounded = true;
                if (direction.ToString() == "Left") controller.wallLeft = true;
                if (direction.ToString() == "Right") controller.wallRight = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            colliders--;
            if (colliders < 0) colliders = 0;
            if (colliders == 0)
            {
                if (direction.ToString() == "Down") controller.grounded = false;
                if (direction.ToString() == "Left") controller.wallLeft = false;
                if (direction.ToString() == "Right") controller.wallRight = false;
            }
        }
    }
}
