using Photon;
using UI.Combat.Stats;
using UnityEditor;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("UI - Local player")]
        [SerializeField] private RectTransform mainButtonSection;
        [SerializeField] private RectTransform secondaryButtonSection;
        [SerializeField] private ShootButton shootButton;
        [SerializeField] private ReloadButton reloadButton;
        [SerializeField] private HealthDisplayer playerHealth;
        [SerializeField] private EnergyDisplayer energyDisplayer;
        [SerializeField] private BulletDisplayer bulletDisplayer;
        [SerializeField] private OutOfLimitsWarning outOfLimitsWarning;

        [Header("Components")]
        [SerializeField] private Fader fader;

        private bool _reloadingButtonInMainSection;

        public void StartCombatPhase()
        {
            fader.FadeIn();
            var localPlayer = PhotonRoom.Instance.LocalPlayer;
            localPlayer.Shooter.OnAmmoChange += AmmoChange;
            localPlayer.Shooter.OnStopReloading += StopReloading;
            playerHealth.DisplayHealth(localPlayer);
            energyDisplayer.DisplayEnergy(localPlayer);
            shootButton.Show();
            reloadButton.Show();
            bulletDisplayer.Show();
            outOfLimitsWarning.Show();
        }

        private void AmmoChange(int newAmmo)
        {
            if (newAmmo != 0) return;
            shootButton.gameObject.SetActive(false);
            reloadButton.transform.SetParent(mainButtonSection, false);
            _reloadingButtonInMainSection = true;
        }

        private void StopReloading()
        {
            if (!_reloadingButtonInMainSection) return;
            shootButton.gameObject.SetActive(true);
            reloadButton.transform.SetParent(secondaryButtonSection, false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor (typeof(PlayerUI))]
    public class PlayerUIEditor : Editor {
        public override void OnInspectorGUI () {
            var playerUI = (PlayerUI)target;
            if(GUILayout.Button("Show/Hide"))
            {
                if (playerUI.TryGetComponent<CanvasGroup>(out var canvasGroup))
                {
                    if (canvasGroup.interactable)
                    {
                        canvasGroup.alpha = 0;
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    }
                    else
                    {
                        canvasGroup.alpha = 1;
                        canvasGroup.interactable = true;
                        canvasGroup.blocksRaycasts = true;
                    }
                }
            }
            DrawDefaultInspector ();
        }
    }
#endif
}
