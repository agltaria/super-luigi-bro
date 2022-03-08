using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class CastleController : MonoBehaviour
    {
        [SerializeField] private float levelEndWait;
        [SerializeField] private Timer timer;
        [SerializeField] private float timerDrainSecondDuration;

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
            
            // TODO: Raise the flag
            // TODO: Wait for a few seconds
            // TODO: End the level
        }
    }
}