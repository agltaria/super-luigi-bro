using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float startTime;
        [SerializeField] private float gameSecondLength;

        private float _currentTime;
        private bool timerActive;

        public UnityEvent timeChanged = new UnityEvent();
        
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                timeChanged.Invoke();
            }
        }

        private void Start()
        {
            CurrentTime = startTime;
            
            // TODO: Start timer after the opening transition
            StartTimer();
        }

        private void Update()
        {
            if (!timerActive) return;
            
            CurrentTime -= Time.deltaTime / gameSecondLength;

            if (!(CurrentTime <= 0)) return;
            
            CurrentTime = 0;

            StopTimer();
            if(PlayerDeath.Instance)
                PlayerDeath.Instance.Die();
        }
        
        public void ResetTimer()
        {
            CurrentTime = startTime;
        }

        public void StartTimer()
        {
            timerActive = true;
        }

        public void StopTimer()
        {
            timerActive = false;
        }
    }
}