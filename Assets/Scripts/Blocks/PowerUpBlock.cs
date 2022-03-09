using UnityEngine;

namespace Blocks
{
    public class PowerUpBlock : Block
    {
        [SerializeField] private Sprite triggeredSprite;
        [SerializeField] private GameObject blockPowerUpPrefab;
        [SerializeField] private GameObject powerUpPrefab;
        [SerializeField] private bool isProgressive;
        [SerializeField] private GameObject progressivePowerUpPrefab;

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
                
            if (isProgressive && PlatformerPlayer.CurrentForm != PlatformerPlayer.MarioForm.Small)
                blockPowerUp.GetComponent<BlockPowerUp>().powerUpPrefab = progressivePowerUpPrefab;
            else
                blockPowerUp.GetComponent<BlockPowerUp>().powerUpPrefab = powerUpPrefab;

            enabled = false;
        }
    }
}