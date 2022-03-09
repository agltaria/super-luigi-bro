using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Animator))]
    public class CastleController : MonoBehaviour
    {
        [SerializeField] private float levelEndWait;
        [SerializeField] private Timer timer;
        [SerializeField] private float timerDrainSecondDuration;

        private Animator animator;
        
        private static readonly int RaiseFlag = Animator.StringToHash("raiseFlag");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            
            col.gameObject.SetActive(false);

            StartCoroutine(EndSequence());
        }

        private IEnumerator EndSequence()
        {
            while (timer.CurrentTime > 0)
            {
                timer.CurrentTime -= 1;

                if (timer.CurrentTime < 0)
                {
                    timer.CurrentTime = 0;
                }
                
                // TODO: Add score
                
                yield return new WaitForSeconds(timerDrainSecondDuration);
            }
            
            animator.SetTrigger(RaiseFlag);
            
            yield return new WaitForSeconds(levelEndWait);
            
            // TODO: End the level
        }
    }
}