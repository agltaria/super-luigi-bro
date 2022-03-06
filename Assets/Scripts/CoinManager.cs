using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private int coinsForOneUp;

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
        private int _coins;

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