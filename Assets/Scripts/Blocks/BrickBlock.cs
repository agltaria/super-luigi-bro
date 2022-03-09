using DefaultNamespace;
using UnityEngine;

namespace Blocks
{
    public class BrickBlock : Block
    {
        [SerializeField] private BrickDebris brickDebris;
        
        private SpriteRenderer spriteRenderer;
        private Collider2D collider;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        [ContextMenu("Trigger")]
        public override void Trigger()
        {
            if (PlatformerPlayer.currentForm is not (PlatformerPlayer.MarioForm.Big or PlatformerPlayer.MarioForm.Fire)) return;
            
            spriteRenderer.enabled = false;
            collider.enabled = false;

            brickDebris.Trigger();
        }
    }
}