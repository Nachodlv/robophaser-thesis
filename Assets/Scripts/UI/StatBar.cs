using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField][Tooltip("Current value text")]
        private TextMeshProUGUI currentValueText;
        [SerializeField] [Tooltip("Max value text")]
        private TextMeshProUGUI maxValueText;
        [SerializeField] [Tooltip("Image of the scrollbar")]
        private Image statBarImage;
        [SerializeField][Tooltip("Max value of the stat bar")]
        private float maxValue;
        [SerializeField][Tooltip("Current value of the stat bar")]
        private float currentValue;
        [SerializeField] private bool showText = true;
        [SerializeField] private Color maxColor;
        [SerializeField] private Color minColor;

        /// <summary>
        /// When the CurrentValue is set then it updates the fillAmount of the statBarImage and the text of the currentValue.
        /// </summary>
        public float CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                if(showText) currentValueText.text = value.ToString(CultureInfo.InvariantCulture);

                var barValue =  currentValue / (float) maxValue;
                statBarImage.fillAmount = barValue;
                statBarImage.color = Color.Lerp(minColor, maxColor, currentValue / maxValue);
            }
        }

        /// <summary>
        /// When the MaxValue is set then it updates the text of the maxValueText.
        /// </summary>
        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                if(showText) maxValueText.text = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
