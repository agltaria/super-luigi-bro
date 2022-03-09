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
        [SerializeField] private Text text;
        //private TextMeshProUGUI text;
        //private Text text;

        private void Awake()
        {
            coinManager.coinsChanged.AddListener(OnCoinsChanged);
            text.text = coinManager.Coins.ToString("00"); //testing, comment this out later

            //text = GetComponent<TextMeshProUGUI>();
            //text = GetComponent<Text>();
        }
        private void Update()
        {
        }
        private void OnCoinsChanged()
        {
            text.text = coinManager.Coins.ToString("00");
        }
    }
}