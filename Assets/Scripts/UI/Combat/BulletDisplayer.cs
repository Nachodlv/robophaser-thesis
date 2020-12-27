using Photon;
using TMPro;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class BulletDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI maxBullets;
        [SerializeField] private TextMeshProUGUI currentBullets;
        [SerializeField] private TextAutoSizeController autoSizeController;

        public void Show()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            maxBullets.text = shooter.MaxClipAmmo.ToString();
            currentBullets.text = maxBullets.text;
            autoSizeController.RefreshAutoSize();
            shooter.OnAmmoChange += AmmoChange;
        }

        private void AmmoChange(int newAmmo)
        {
            currentBullets.text = newAmmo.ToString();
            autoSizeController.RefreshAutoSize();
        }
    }
}
