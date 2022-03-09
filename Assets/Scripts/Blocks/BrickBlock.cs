using DefaultNamespace;
using UnityEngine;

namespace Blocks
{
    public class BrickBlock : Block
    {
        [SerializeField] private BrickDebris brickDebris;
        [SerializeField] private int breakingScore;
        
        private SpriteRenderer spriteRenderer;
        private new Collider2D collider;

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        [ContextMenu("Trigger")]
        public override void Trigger()
        {
            base.Trigger();
            
            if (PlatformerPlayer.CurrentForm is not (PlatformerPlayer.MarioForm.Big or PlatformerPlayer.MarioForm.Fire)) return;
            
            spriteRenderer.enabled = false;
            collider.enabled = false;
            
            ScoreManager.Instance.AddScore(breakingScore);

            brickDebris.Trigger();
        }
    }
}