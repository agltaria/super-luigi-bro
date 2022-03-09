using UnityEngine;

namespace DefaultNamespace
{
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            
            PlayerDeath.Instance.Die();
        }
    }
}