using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private Timer timer;

        private TextMeshProUGUI text;

        private void Awake()
        {
            timer.timeChanged.AddListener(OnTimeChanged);

            text = GetComponent<TextMeshProUGUI>();
        }

        private void OnTimeChanged()
        {
            text.text = timer.CurrentTime.ToString("F0");
        }
    }
}