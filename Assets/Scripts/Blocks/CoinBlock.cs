using UnityEngine;

namespace Blocks
{
    public class CoinBlock : Block
    {
        [SerializeField] private Sprite triggeredSprite;
        [SerializeField] private int numberOfCoins;
        [SerializeField] private GameObject blockCoinPrefab;
        [SerializeField] private Vector2 spawnOffset;

        private SpriteRenderer spriteRenderer;
        private int coinsSpawned;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        [ContextMenu("Trigger")]
        protected override void Trigger()
        {
            if (coinsSpawned >= numberOfCoins - 1)
                spriteRenderer.sprite = triggeredSprite;
            
            Instantiate(blockCoinPrefab, transform.position + (Vector3) spawnOffset, Quaternion.identity, transform);
            
            coinsSpawned++;
            
            Bump();
        }

        protected override void OnBumpComplete()
        {
            base.OnBumpComplete();
            
            // TODO: Update coin count
            
            if (coinsSpawned >= numberOfCoins)
                enabled = false;
        }
    }
}