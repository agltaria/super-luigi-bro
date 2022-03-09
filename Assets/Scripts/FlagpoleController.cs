using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

namespace DefaultNamespace
{
    public class FlagpoleController : MonoBehaviour
    {
        [Serializable]
        struct ScoreZone
        {
            public int score;
            public float minYPos;
        }
        
        [SerializeField] private Transform flag;
        [SerializeField] private float minYPos;
        [SerializeField] private AnimatorController smallMarioAnimatorController;
        [SerializeField] private AnimatorController bigMarioAnimatorController;
        [SerializeField] private AnimatorController fireMarioAnimatorController;
        [SerializeField] private float slideSpeed;
        [SerializeField] private Timer timer;
        [SerializeField] private List<ScoreZone> scoreZones;

        private Transform playerTransform;
        private PlatformerPlayer playerMovement;
        private Animator playerAnimator;

        private bool playerSliding;
        private bool flagSliding;
        
        private static readonly int FlagpoleJump = Animator.StringToHash("flagpoleJump");

        private void Update()
        {
            Slide(playerTransform, ref playerSliding, () => { });
            Slide(flag, ref flagSliding, OnFlagSlideComplete);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            CalculateScore(other.transform);

            playerTransform = other.transform;
            playerMovement = other.GetComponent<PlatformerPlayer>();
            playerAnimator = other.GetComponent<Animator>();

            playerMovement.enabled = false;

            switch (PlatformerPlayer.CurrentForm)
            {
                case PlatformerPlayer.MarioForm.Small:
                    playerAnimator.runtimeAnimatorController = smallMarioAnimatorController;
                    break;
                case PlatformerPlayer.MarioForm.Big:
                    playerAnimator.runtimeAnimatorController = bigMarioAnimatorController;
                    break;
                case PlatformerPlayer.MarioForm.Fire:
                    playerAnimator.runtimeAnimatorController = fireMarioAnimatorController;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            playerAnimator.applyRootMotion = true;
            playerAnimator.speed = 1;
            
            timer.StopTimer();

            playerSliding = true;
            flagSliding = true;
        }

        private void CalculateScore(Transform playerTransform)
        {
            var yPos = playerTransform.position.y;

            foreach (var scoreZone in scoreZones)
            {
                if (yPos > scoreZone.minYPos)
                {
                    Debug.Log(scoreZone.score);
                    ScoreManager.Instance.AddScore(scoreZone.score);
                    return;
                }
            }
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
            playerAnimator.SetTrigger(FlagpoleJump);
        }
    }
}