using Photon.Combat;
using Photon.GameControllers;
using UnityEngine;

namespace UI.Combat.Stats
{
    public class EnergyDisplayer : MonoBehaviour
    {
        [SerializeField] private StatBarSlider slider;

        public void DisplayEnergy(PhotonPlayer player)
        {
            var wallPhaser = player.CameraTransform.GetComponentInChildren<WallPhaser>();
            slider.MaxValue = wallPhaser.MaxEnergy;
            slider.Value = wallPhaser.MaxEnergy;
            wallPhaser.OnEnergyUpdate += EnergyUpdated;
        }

        private void EnergyUpdated(float energy)
        {
            slider.Value = energy;
        }
    }
}
