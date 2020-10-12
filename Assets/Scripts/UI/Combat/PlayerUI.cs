using System.Threading.Tasks;
using Photon;
using Photon.GameControllers;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private ShootButton shootButton;
        [SerializeField] private Fader fader;

        public void StartCombatPhase()
        {
            fader.FadeIn();
            shootButton.Show(PhotonRoom.Instance.LocalPlayer.Shooter);
        }
    }
}
