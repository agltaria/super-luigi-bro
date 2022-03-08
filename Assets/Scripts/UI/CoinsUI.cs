using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] private CoinManager coinManager;

        private TextMeshProUGUI text;

        private void Awake()
        {
            coinManager.coinsChanged.AddListener(OnCoinsChanged);

            text = GetComponent<TextMeshProUGUI>();
        }

        private void OnCoinsChanged()
        {
            text.text = coinManager.Coins.ToString("00");
        }
    }
}