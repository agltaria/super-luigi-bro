using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BrickDebris : MonoBehaviour
    {
        [SerializeField] private List<BrickDebrisPart> parts;
        [SerializeField] private float force;
        [SerializeField] private float destroyAfter;
        
        private new Rigidbody2D rigidbody;

        private bool trigger;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;
            
            parts.ForEach(part => part.gameObject.SetActive(false));
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            trigger = true;

            StartCoroutine(Destroy());
        }

        private void FixedUpdate()
        {
            if (!trigger) return;
            
            parts.ForEach(part =>
            {
                part.gameObject.SetActive(true);
            });
            
            rigidbody.AddForce(Vector2.up * force);
            
            rigidbody.gravityScale = 1f;

            trigger = false;
        }

        private IEnumerator Destroy()
        {
            AudioManager.audioManager.playSound(3);
            yield return new WaitForSeconds(destroyAfter);

            gameObject.SetActive(false);
        }
    }
}