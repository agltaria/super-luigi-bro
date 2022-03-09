using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class FlagpoleController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D flagpoleCollider;
        [SerializeField] private BoxCollider2D castleDoorCollider;
        [SerializeField] private float minYPos;

        [SerializeField] private float slideSpeed;

        private Transform player;
        private Transform flag;

        private bool playerSliding;
        private bool flagSliding;
        
        private void Update()
        {
            Slide(player, ref playerSliding, () => { });
            Slide(flag, ref flagSliding, OnFlagSlideComplete);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
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
            
            transform.position -= Vector3.down * slideSpeed * Time.deltaTime;

            if (!(transform.position.y < minYPos)) return;
            
            var temp = transform.position;
            temp.x = minYPos;
            transform.position = temp;

            isSliding = false;
            
            onComplete.Invoke();
        }

        private void OnFlagSlideComplete()
        {
            playerSliding = false;

            // TODO: Flip player sprite
            // TODO: Small delay
            // TODO: Backward jump off flagpole
            // TODO: Start player walking right
        }
    }
}