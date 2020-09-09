using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARCore
{
    public class GameArea : MonoBehaviour
    {
        [SerializeField] private Slider xSlider;
        [SerializeField] private Slider zSlider;

        private void Awake()
        {
            var localScale = transform.localScale;
            xSlider.value = localScale.x;
            zSlider.value = localScale.z;

            xSlider.onValueChanged.AddListener(ScaleInX);
            zSlider.onValueChanged.AddListener(ScaleInZ);
        }

        private void ScaleInX(float scale)
        {
            var transform1 = transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.x = scale;
            transform1.localScale = transformLocalScale;
        }

        private void ScaleInZ(float scale)
        {
            var transform1 = transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.z = scale;
            transform1.localScale = transformLocalScale;
        }
    }
}
