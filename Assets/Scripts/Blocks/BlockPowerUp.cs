using UnityEngine;

namespace Blocks
{
    public class BlockPowerUp : MonoBehaviour
    {
        public GameObject powerUpPrefab;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            var powerUpSprite = powerUpPrefab.GetComponent<SpriteRenderer>().sprite;

            spriteRenderer.sprite = powerUpSprite;
        }

        private void Destroy()
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity, transform.parent);
            
            Destroy(gameObject);
        }
    }
}