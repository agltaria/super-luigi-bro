using DefaultNamespace;
//using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    //[RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(Text))]
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] private CoinManager coinManager;

        //private TextMeshProUGUI text;
        private Text text;

        private void Awake()
        {
            coinManager.coinsChanged.AddListener(OnCoinsChanged);

            //text = GetComponent<TextMeshProUGUI>();
            text = GetComponent<Text>();
        }

        private void OnCoinsChanged()
        {
            text.text = coinManager.Coins.ToString("00");
        }
    }
}