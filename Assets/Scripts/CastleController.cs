using UnityEngine;

namespace DefaultNamespace
{
    public class CastleController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            
            Destroy(col.gameObject);
            
            // TODO: Disable player input
            // TODO: Hide player sprite
            // TODO: Start converting time to score
        }

        private void OnTimeConversionComplete()
        {
            // TODO: Raise the flag
            // TODO: Wait for a few seconds
            // TODO: End the level
        }
    }
}