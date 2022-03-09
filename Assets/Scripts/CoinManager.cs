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
                DestroyImmediate(this.gameObject);
            }
        }

        private void Start()
        {
            Coins = 0;
            coinsChanged.Invoke();

        }

        public void AddCoin()
        {
            Coins++;
            coinsChanged.Invoke();
            
            ScoreManager.Instance.AddScore(200);

            if (Coins >= coinsForOneUp)
            {
                Coins -= coinsForOneUp;
                PlayerPrefs.SetInt("MarioLives", PlayerPrefs.GetInt("MarioLives",1));
            }
        }
    }
}