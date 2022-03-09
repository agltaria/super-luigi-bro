using DefaultNamespace;
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
        private Animator spriteAnimator;
        private int coinsSpawned;
        private CoinManager coinManager;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteAnimator = spriteRenderer.GetComponent<Animator>();
        }

        protected override void Start()
        {
            base.Start();
            
            coinManager = CoinManager.Instance;
        }

        [ContextMenu("Trigger")]
        public override void Trigger()
        {
            if (Triggered) return;
            
            base.Trigger();
            
            if (coinsSpawned >= numberOfCoins - 1)
            {
                spriteAnimator.enabled = false;
                spriteRenderer.sprite = triggeredSprite;
            }

            Instantiate(blockCoinPrefab, transform.position + (Vector3) spawnOffset, Quaternion.identity, transform);
            
            coinsSpawned++;
            
            Bump();
        }

        protected override void OnBumpComplete()
        {
            base.OnBumpComplete();
            
            coinManager.AddCoin();

            if (coinsSpawned >= numberOfCoins)
                Triggered = true;
        }
    }
}