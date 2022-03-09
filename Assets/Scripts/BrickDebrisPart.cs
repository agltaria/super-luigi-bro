using UnityEngine;

namespace DefaultNamespace
{
    public class BrickDebrisPart : MonoBehaviour
    {
        [SerializeField] private Vector2 direction;
        
        private void Update()
        {
            transform.position += (Vector3) direction * Time.deltaTime;
        }
    }
}