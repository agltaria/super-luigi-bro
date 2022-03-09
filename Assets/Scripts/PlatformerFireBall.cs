using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PlatformerFireBall : PlatformerPhysics
{
    bool isExploding;
    [SerializeField] float xSpeed = 7.0f;
    [SerializeField] float bounceForce = 15.0f;
    [SerializeField] float cameraBounds = 12.0f;
    int direction;
    bool bufferedBounce;
    Transform camera;
    Vector3 offset = new Vector3(0, 0, -10.0f);

    protected override void Awake()
    {
        base.Awake();
        camera = Camera.main.transform;
    }

    void Update()
    {
        if (bufferedBounce)
        {
            bufferedBounce = false;
            velocity =new Vector2(velocity.x, bounceForce);
        }

        // Destroy if out of bounds
        if ((camera.position - (transform.position + offset)).magnitude > cameraBounds)
        {
            FindObjectOfType<PlatformerPlayer>().fireBalls++;
            Destroy(gameObject);
        }
    }

    protected override void FixedUpdate()
    {
        if (!isExploding) base.FixedUpdate();
        // Debug.Log(isGrounded);
    }

    public void SetDirection(int direction)
    {
        this.direction = direction;
        velocity = new Vector2(direction * xSpeed, velocity.y);
    }

    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        // Debug.Log("Hit! " + direction);

        base.HitWall(direction, hit);

        if (!isExploding)
        {
            //PlatformerEnemy enemy = hit.collider.gameObject.GetComponent<PlatformerEnemy>();
            //if (enemy != null)
            //{
            //    // Hurt enemy and explode
            //    enemy.GetHurt(false);
            //    GetComponent<Animator>().SetTrigger("explode");
            //    isExploding = true;
            //}
            //else
            if (hit.collider.gameObject.tag == "Untagged")
            {
                switch (direction)
                {
                    case 2:
                        // Explode
                        GetComponent<Animator>().SetTrigger("explode");
                        isExploding = true;
                        break;
                    case 3:
                        // Explode
                        GetComponent<Animator>().SetTrigger("explode");
                        isExploding = true;
                        break;
                    default:
                        break;
                }
            }
        }

        if (direction == 0) bufferedBounce = true;
    }

    public void DestroyFireBall()
    {
        FindObjectOfType<PlatformerPlayer>().fireBalls++;
        Destroy(gameObject);
    }
}
