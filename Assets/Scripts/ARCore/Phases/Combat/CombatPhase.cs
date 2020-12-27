using Photon;
using Photon.GameControllers;
using UI;
using UI.Combat;
using UnityEngine;

namespace ARCore.Phases.Combat
{
    public class CombatPhase: Phase
    {
        private readonly PlayerUI _playerUI;
        private readonly EndGameScreen _defeatScreen;
        private bool _gameEnded;

        public CombatPhase(PhaseManager phaseManager, PlayerUI playerUI, EndGameScreen defeatScreen) : base(phaseManager)
        {
            _playerUI = playerUI;
            _defeatScreen = defeatScreen;
        }

        public override void OnEnter()
        {
            _playerUI.StartCombatPhase();
            foreach (var player in PhotonRoom.Instance.PhotonPlayers)
            {
                player.OnHealthUpdate += health => PlayerHealthUpdated(player, health);
            }
        }

        private void PlayerHealthUpdated(PhotonPlayer player, int newHealth)
        {
            if (newHealth > 0 || _gameEnded) return;

            // it would be cleaner to remove the callback from the event and not having the variable _gameEnded
            _gameEnded = true;
            if (player.photonView.IsMine)
            {
                _defeatScreen.ShowDefeat();
            }
            else
            {
                _defeatScreen.ShowVictory();
            }
            // else show win screen
        }
    }
}
