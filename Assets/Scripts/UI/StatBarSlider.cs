using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatBarSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Color minimumColor;
        [SerializeField] private Color middleColor;
        [SerializeField] private Color maximumColor;

        public float MaxValue
        {
            get => slider.maxValue;
            set => slider.maxValue = value;
        }

        public float Value
        {
            get => slider.value;
            set
            {
                var fiftyPercent = slider.maxValue / 2;
                SetSliderColor(value >= fiftyPercent
                    ? Color.Lerp(middleColor, maximumColor, (value - fiftyPercent) / fiftyPercent)
                    : Color.Lerp(minimumColor, middleColor, value / fiftyPercent));

                slider.value = value;
            }
        }

        private void SetSliderColor(Color color)
        {
            var sliderColors = slider.colors;
            sliderColors.normalColor = color;
            slider.colors = sliderColors;
        }
    }
}
