using UnityEngine;

namespace Blocks
{
    public class BlockCoin : MonoBehaviour
    {
        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
