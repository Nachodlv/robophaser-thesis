using UnityEngine;
using WFC;

namespace UI.Combat
{
    public class OutOfLimitsWarning : MonoBehaviour
    {
        [SerializeField] private GameObject warning;
        public void Show()
        {
            var mapLimits = FindObjectOfType<MapLimits>();
            mapLimits.LocalPlayerOutOfLimits += ShowWarning;
            mapLimits.LocalPlayerInsideLimits += HideWarning;
        }

        private void ShowWarning()
        {
            warning.SetActive(true);
        }

        private void HideWarning()
        {
            warning.SetActive(false);
        }
    }
}
