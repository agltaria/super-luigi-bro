using DefaultNamespace;
//using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private Timer timer;
        [SerializeField] private Text text;
        //private TextMeshProUGUI text;


        private void Awake()
        {
            timer.timeChanged.AddListener(OnTimeChanged);
            text.text = "Time";
            //text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            //.text = string.Format("Time/n" + timer.CurrentTime.ToString("000")); //testing, comment out later
        }

        private void OnTimeChanged()
        {
            text.text = string.Format("Time" + "\n" + timer.CurrentTime.ToString("000"));//timer.CurrentTime.ToString("000");
        }
    }
}