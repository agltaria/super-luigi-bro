using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            else
            {
                // Debug.LogWarning($"More than one {nameof(PlayerDeath)}.");
                enabled = false;
            }
        }

        [ContextMenu("Die")]
        public void Die()
        {
            // Debug.Log("die");
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            playerMovement.enabled = false;
            animator.enabled = false;
            collider.enabled = false;
            Time.timeScale = 0;
            
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

            yield return new WaitForSecondsRealtime(waitBeforeJump);

            float timer = 0;
            float speed = jumpForce;

            while (timer < waitBeforeRestart)
            {
                speed += -9.8f * jumpGravity * Time.unscaledDeltaTime;
                transform.position += Vector3.up * speed * Time.unscaledDeltaTime;
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            
            PlayerPrefs.SetInt("MarioLives",PlayerPrefs.GetInt("MarioLives") - 1);
            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync("LevelStartScreen", LoadSceneMode.Additive);
            // TODO: Restart level
        }
    }
}