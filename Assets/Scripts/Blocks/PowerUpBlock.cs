using UnityEngine;

namespace Blocks
{
    public class PowerUpBlock : Block
    {
        [SerializeField] private Sprite triggeredSprite;
        [SerializeField] private GameObject blockPowerUpPrefab;

        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        [ContextMenu("Trigger")]
        protected override void Trigger()
        {
            spriteRenderer.sprite = triggeredSprite;
            
            Bump();
        }

        protected override void OnBumpComplete()
        {
            base.OnBumpComplete();

            Instantiate(blockPowerUpPrefab, transform);

            enabled = false;
        }
    }
}