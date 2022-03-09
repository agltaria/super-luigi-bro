using DefaultNamespace;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private CoinManager coinManager;

    private void Start()
    {
        coinManager = CoinManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Coin")) return;
        
        coinManager.AddCoin();
        
            
        Destroy(col.gameObject);
    }
}
