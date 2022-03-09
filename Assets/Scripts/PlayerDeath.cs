using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(BoxCollider2D), typeof(PlatformerPlayer), typeof(Animator))]
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField] private Sprite normalDeathSprite;
        [SerializeField] private Sprite fireDeathSprite;
        [SerializeField] private float waitBeforeJump;
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpGravity;
        [SerializeField] private float waitBeforeRestart;

        private new BoxCollider2D collider;
        private PlatformerPlayer playerMovement;
        private Animator animator;
        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;

        private bool trigger;

        public static PlayerDeath Instance;
        
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                collider = GetComponent<BoxCollider2D>();
                playerMovement = GetComponent<PlatformerPlayer>();
                animator = GetComponent<Animator>();
                rigidbody = GetComponent<Rigidbody2D>();
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            else
            {
                Debug.LogWarning($"More than one {nameof(PlayerDeath)}.");
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (!trigger) return;
            
            rigidbody.AddForce(jumpForce * Vector2.up);

            trigger = false;
        }

        [ContextMenu("Die")]
        public void Die()
        {
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            playerMovement.enabled = false;
            animator.enabled = false;
            collider.enabled = false;
            
            switch (PlatformerPlayer.CurrentForm)
            {
                case PlatformerPlayer.MarioForm.Small:
                case PlatformerPlayer.MarioForm.Big:
                    spriteRenderer.sprite = normalDeathSprite;
                    break;
                case PlatformerPlayer.MarioForm.Fire:
                    spriteRenderer.sprite = fireDeathSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(waitBeforeJump);
            
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = jumpGravity;
            trigger = true;

            yield return new WaitForSeconds(waitBeforeRestart);

            // TODO: Restart level
        }
    }
}