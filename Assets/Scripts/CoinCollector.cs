using DefaultNamespace;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Coin")) return;
        
        coinManager.AddCoin();
            
        Destroy(col.gameObject);
    }
}
