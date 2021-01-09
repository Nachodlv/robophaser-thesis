using Photon;
using Photon.GameControllers;
using Photon.Pun;
using UI;
using UI.Combat;

namespace ARCore.Phases.Combat
{
    public class CombatPhase: Phase
    {
        private readonly PlayerUI _playerUI;
        private readonly EndGameScreen _defeatScreen;
        private readonly bool _skipCombat;
        private bool _gameEnded;

        public CombatPhase(PhaseManager phaseManager, PlayerUI playerUI, bool skipCombat) : base(phaseManager)
        {
            _playerUI = playerUI;
            _defeatScreen = phaseManager.EndGameScreen;
            _skipCombat = skipCombat;
        }

        public override void OnEnter()
        {
            _playerUI.StartCombatPhase();
            foreach (var player in PhotonRoom.Instance.PhotonPlayers)
            {
                player.OnHealthUpdate += health => PlayerHealthUpdated(player, health);
            }

            if (_skipCombat && PhotonNetwork.IsMasterClient) KillRandomPlayer();
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
        }

        public override void OpponentLeft()
        {
            if (!_gameEnded)
            {
                base.OpponentLeft();
                return;
            }
            _defeatScreen.DisableRematch();
        }

        private void KillRandomPlayer()
        {
            var players = PhotonRoom.Instance.PhotonPlayers;
            var randomIndex = UnityEngine.Random.Range(0, players.Count);
            players[randomIndex].ReceiveDamage(players[randomIndex].MaxHealth);
        }
    }
}
