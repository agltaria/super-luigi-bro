using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class FlagpoleController : MonoBehaviour
    {
        [SerializeField] private Transform flag;
        [SerializeField] private BoxCollider2D flagpoleCollider;
        [SerializeField] private BoxCollider2D castleDoorCollider;
        [SerializeField] private float minYPos;
        [SerializeField] private float jumpDelay;
        [SerializeField] private float animatorSpeed;

        [SerializeField] private float slideSpeed;

        private Transform player;
        private PlatformerPlayer playerMovement;
        private Animator playerAnimator;
        private SpriteRenderer playerSprite;

        private bool playerSliding;
        private bool flagSliding;
        private bool playerMoving;
        private static readonly int FlagpoleSlide = Animator.StringToHash("flagpoleSlide");
        private static readonly int FlagpoleJump = Animator.StringToHash("flagpoleJump");
        private static readonly int InJump = Animator.StringToHash("inJump");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void Update()
        {
            Slide(player, ref playerSliding, () => { });
            Slide(flag, ref flagSliding, () => OnFlagSlideComplete());

            if (!playerMoving) return;

            player.position += Vector3.right * slideSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            player = other.transform;
            
            playerMovement = other.GetComponent<PlatformerPlayer>();
            playerSprite = other.GetComponent<SpriteRenderer>();

            playerMovement.enabled = false;

            playerAnimator = other.GetComponent<Animator>();
            
            playerAnimator.SetTrigger(FlagpoleSlide);

            // TODO: Disable player input
            // TODO: Disable player physics
            // TODO: Stop timer
            // TODO: Set player sprite to grab

            playerSliding = true;
            flagSliding = true;
        }

        private void Slide(Transform transform, ref bool isSliding, Action onComplete)
        {
            if (!isSliding) return;
            
            transform.position += Vector3.down * slideSpeed * Time.deltaTime;

            if (!(transform.position.y < minYPos)) return;
            
            var temp = transform.position;
            temp.y = minYPos;
            transform.position = temp;

            isSliding = false;
            
            onComplete.Invoke();
        }

        private void OnFlagSlideComplete()
        {
            // playerSliding = false;
            //
            playerAnimator.SetTrigger(FlagpoleJump);
            //
            // playerSprite.flipX = true;
            //
            // yield return new WaitForSeconds(jumpDelay);
            //
            // // playerMovement.enabled = true;
            //
            // playerAnimator.SetBool(InJump, true);
            //
            // playerMoving = true;
            //
            // yield return new WaitForSeconds(jumpDelay);
            //
            // playerSprite.flipX = false;
            //
            // playerAnimator.SetBool(InJump, false);
            // playerAnimator.SetBool(IsGrounded, true);
            // playerAnimator.SetBool(IsMoving, true);
            //
            // playerAnimator.speed = animatorSpeed;

            // TODO: Flip player sprite
            // TODO: Small delay


            // TODO: Backward jump off flagpole
            // TODO: Start player walking right
        }
    }
}