using UnityEngine;

namespace Blocks
{
    public class BlockPowerUp : MonoBehaviour
    {
        [SerializeField] private GameObject powerUpPrefab;
        
        private void Destroy()
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity, transform.parent);
            
            Destroy(gameObject);
        }
    }
}