using UnityEngine;

namespace Blocks
{
    public class PowerUpBlock : Block
    {
        [SerializeField] private Sprite triggeredSprite;
        [SerializeField] private GameObject blockPowerUpPrefab;
        [SerializeField] private GameObject powerUpPrefab;

        private SpriteRenderer spriteRenderer;
        private Animator spriteAnimator;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteAnimator = spriteRenderer.GetComponent<Animator>();
        }

        [ContextMenu("Trigger")]
        public override void Trigger()
        {
            spriteAnimator.enabled = false;
            spriteRenderer.sprite = triggeredSprite;
            
            Bump();
        }

        protected override void OnBumpComplete()
        {
            base.OnBumpComplete();

            var blockPowerUp = Instantiate(blockPowerUpPrefab, transform);

            blockPowerUp.GetComponent<BlockPowerUp>().powerUpPrefab = powerUpPrefab;

            enabled = false;
        }
    }
}