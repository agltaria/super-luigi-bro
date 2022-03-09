using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    [RequireComponent(typeof(Animator))]
    public class Block : MonoBehaviour
    {
        private Animator animator;
        private static readonly int BumpAnimParam = Animator.StringToHash("Bump");

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Start() { }

        public virtual void Trigger() { }

        protected void Bump()
        {
            animator.SetTrigger(BumpAnimParam);
        }
        
        // Called by the end of the bump animation
        protected virtual void OnBumpComplete() { }
    }
}