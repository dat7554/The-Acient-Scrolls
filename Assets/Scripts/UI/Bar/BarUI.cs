using UnityEngine;
using UnityEngine.UI;

namespace UI.Bar
{
    public class BarUI : MonoBehaviour
    {
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void SetSliderValue(float value)
        {
            _slider.value = value;
        }
    }
}
