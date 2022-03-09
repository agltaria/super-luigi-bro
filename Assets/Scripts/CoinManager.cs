using System;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private int coinsForOneUp;

        public static CoinManager Instance;

        public int Coins
        {
            get => _coins;
            private set
            {
                _coins = value;
                coinsChanged.Invoke();
            }
        }

        public UnityEvent coinsChanged = new UnityEvent();
        [SerializeField] private int _coins;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning($"More than one {nameof(CoinManager)}.");
                enabled = false;
            }
        }

        private void Start()
        {
            Coins = 0;
        }

        public void AddCoin()
        {
            Coins++;

            if (Coins >= coinsForOneUp)
            {
                Coins -= coinsForOneUp;
                
                // TODO: One up
            }
        }
    }
}