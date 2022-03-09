using UnityEngine;

namespace Blocks
{
    [RequireComponent(typeof(Animator))]
    public class Block : MonoBehaviour
    {
        private Animator animator;
        private static readonly int BumpAnimParam = Animator.StringToHash("Bump");

        protected bool Triggered;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Start() { }

        public virtual void Trigger()
        {
            // Check for enemies above the block
            var hit = Physics2D.Raycast(transform.position + Vector3.up, Vector2.up);

            if (!hit) return;

            if (!hit.collider.CompareTag("Enemy")) return;

            var enemy = hit.collider.GetComponent<PlatformerEnemy>();

            if (enemy == null)
            {
                Debug.LogWarning($"{nameof(GameObject)} with \"Enemy\" tag doesn't have {nameof(PlatformerEnemy)} script." );
                return;
            }
 
            enemy.OnDeath(false, 1);
        }

        protected void Bump()
        {
            AudioManager.audioManager.playExtra(4);
            animator.SetTrigger(BumpAnimParam);
            
        }
        
        // Called by the end of the bump animation
        protected virtual void OnBumpComplete() { }
    }
}