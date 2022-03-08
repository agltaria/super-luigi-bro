using UnityEngine;

namespace Blocks
{
    public class BrickBlock : Block
    {
        private SpriteRenderer spriteRenderer;
        private Collider2D collider;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        [ContextMenu("Trigger")]
        protected override void Trigger()
        {
            // TODO: Check if mario is big
            
            spriteRenderer.enabled = false;
            collider.enabled = false;
            
            // TODO: Spawn brick particles
        }
    }
}