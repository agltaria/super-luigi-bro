using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    private new BoxCollider2D collider;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        collider = GetComponent<BoxCollider2D>();

        float height = mainCamera.orthographicSize * 2;
        float width = height * mainCamera.aspect;

        collider.size = new Vector2(1, height);
        collider.offset = new Vector2(-width / 2 - 0.5f, 0);
    }

    private void Update()
    {
        var transformPosition = transform.position;
        
        if (target.transform.position.x > transformPosition.x)
        {
            transform.position = new Vector3(
                target.transform.position.x,
                transformPosition.y,
                transformPosition.z
            );
        }
    }
}
